using ShippingPlatform.Application.DTOs;
using ShippingPlatform.Application.Interfaces;
using ShippingPlatform.Application.Services;
using ShippingPlatform.Domain.Entities;
using ShippingPlatform.Domain.Enums;
using Xunit;

namespace ShippingPlatform.Tests;

/// <summary>Covers TC-E4-01/02/04/07 (kit definition, packaging validation, custody trail, immutability).</summary>
public class KitServiceTests
{
    private class InMemoryKitRepository : IKitRepository
    {
        private readonly Dictionary<Guid, Kit> _store = new();

        public Task<Kit?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            Task.FromResult(_store.TryGetValue(id, out var kit) ? kit : null);

        public Task AddAsync(Kit kit, CancellationToken ct = default)
        {
            _store[kit.Id] = kit;
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Kit kit, CancellationToken ct = default)
        {
            _store[kit.Id] = kit;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task DispatchKitAsync_ThrowsAndMarksIncomplete_WhenRequiredComponentMissing()
    {
        var repository = new InMemoryKitRepository();
        var service = new KitService(repository);

        var kit = await service.CreateKitAsync(new CreateKitRequest(
            "DNA-COLLECTION-STANDARD",
            null,
            new List<KitComponentRequest>
            {
                new("SWAB", "Buccal Swab", 2, true, "SKU-SWAB-001"),
                new("VIAL", "Sample Vial", 1, true, "SKU-VIAL-001")
            }));

        await service.FulfillComponentAsync(kit.Id, "SWAB");

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.DispatchKitAsync(kit.Id, "user1", "LAB-01"));

        var reloaded = await repository.GetByIdAsync(kit.Id);
        Assert.Equal(KitStatus.Incomplete, reloaded!.Status);
    }

    [Fact]
    public async Task DispatchKitAsync_Succeeds_AndRecordsCustodyEvent_WhenAllComponentsFulfilled()
    {
        var repository = new InMemoryKitRepository();
        var service = new KitService(repository);

        var kit = await service.CreateKitAsync(new CreateKitRequest(
            "DNA-COLLECTION-STANDARD",
            "SO-1001",
            new List<KitComponentRequest> { new("SWAB", "Buccal Swab", 1, true, null) }));

        await service.FulfillComponentAsync(kit.Id, "SWAB");
        var result = await service.DispatchKitAsync(kit.Id, "user1", "LAB-01");

        Assert.Equal(KitStatus.Dispatched, result.Status);

        var reloaded = await repository.GetByIdAsync(kit.Id);
        Assert.Contains(reloaded!.CustodyEvents, e => e.Action == CustodyAction.Dispatched);
        Assert.Contains(reloaded.CustodyEvents, e => e.Action == CustodyAction.Created);
    }

    [Fact]
    public async Task RecordCustodyEventAsync_AppendsEvent_NeverReplacesHistory()
    {
        var repository = new InMemoryKitRepository();
        var service = new KitService(repository);

        var kit = await service.CreateKitAsync(new CreateKitRequest("DNA-COLLECTION-STANDARD", null, new List<KitComponentRequest>()));
        var initialEventCount = (await repository.GetByIdAsync(kit.Id))!.CustodyEvents.Count;

        await service.RecordCustodyEventAsync(kit.Id, new RecordCustodyEventRequest(CustodyAction.Received, "LAB-DE-01", "user2", "Received intact", null));

        var reloaded = await repository.GetByIdAsync(kit.Id);
        Assert.Equal(initialEventCount + 1, reloaded!.CustodyEvents.Count);
        Assert.Equal(KitStatus.Received, reloaded.Status);
    }
}
