using ShippingPlatform.Application.Interfaces;
using ShippingPlatform.Domain.Enums;

namespace ShippingPlatform.Infrastructure.Labels;

/// <summary>
/// Resolves and sends rendered labels to the correct physical lab/warehouse printer (BR-024).
/// Printer resolution is a simple location+label-type lookup here; production implementations
/// typically back this with a printer-mapping table and a print server/queue (e.g. a Zebra
/// print server) rather than direct socket writes from the API process.
/// </summary>
public class PrintRoutingService : IPrintRoutingService
{
    private static readonly Dictionary<(string Location, LabelType Type), string> PrinterMap = new()
    {
        [("DEFAULT", LabelType.Sample)] = "PRN-SAMPLE-01",
        [("DEFAULT", LabelType.CollectionKit)] = "PRN-KIT-01",
        [("DEFAULT", LabelType.Hazard)] = "PRN-HAZARD-01",
    };

    public Task<string> ResolvePrinterIdAsync(string locationCode, LabelType labelType, CancellationToken ct = default)
    {
        var key = (locationCode.ToUpperInvariant(), labelType);
        if (!PrinterMap.TryGetValue(key, out var printerId))
        {
            PrinterMap.TryGetValue(("DEFAULT", labelType), out printerId);
        }

        return Task.FromResult(printerId ?? "PRN-DEFAULT");
    }

    public Task SendToPrinterAsync(string printerId, RenderedLabel label, CancellationToken ct = default)
    {
        // Extension point: forward `label.Content` to the print server/queue identified by printerId.
        return Task.CompletedTask;
    }
}
