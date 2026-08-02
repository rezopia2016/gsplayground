using ShippingPlatform.Application.DTOs;
using ShippingPlatform.Application.Interfaces;
using ShippingPlatform.Domain.Entities;

namespace ShippingPlatform.Application.Services;

/// <summary>
/// Label management service (E3) — covers the six label types D365 does not natively support
/// as a set (customer, internal, sample, collection kit, hazard, return) across ZPL/PDF/QR
/// formats, with full reprint audit trail (BR-020–023).
/// </summary>
public class LabelService : ILabelService
{
    private readonly ILabelRepository _labels;
    private readonly ILabelRenderer _renderer;
    private readonly IPrintRoutingService _printRouting;

    public LabelService(ILabelRepository labels, ILabelRenderer renderer, IPrintRoutingService printRouting)
    {
        _labels = labels;
        _renderer = renderer;
        _printRouting = printRouting;
    }

    public async Task<IReadOnlyList<LabelResponse>> GenerateLabelsAsync(GenerateLabelsRequest request, CancellationToken ct = default)
    {
        var results = new List<LabelResponse>();

        foreach (var type in request.RequestedTypes)
        {
            var renderContext = new LabelRenderContext(
                type,
                request.Format,
                TemplateKey: $"{type}-{request.Format}".ToLowerInvariant(),
                DataFields: new Dictionary<string, string> { ["PackageId"] = request.PackageId.ToString() });

            var rendered = await _renderer.RenderAsync(renderContext, ct);

            var label = new Label
            {
                PackageId = request.PackageId,
                Type = type,
                Format = request.Format,
                TemplateKey = renderContext.TemplateKey,
                BarcodeValue = rendered.BarcodeValue,
                PrintCount = 1
            };
            label.PrintEvents.Add(new LabelPrintEvent { LabelId = label.Id, IsReprint = false });

            await _labels.AddAsync(label, ct);
            results.Add(ToResponse(label));
        }

        return results;
    }

    public async Task<LabelResponse> ReprintAsync(Guid labelId, ReprintLabelRequest request, CancellationToken ct = default)
    {
        var label = await _labels.GetByIdAsync(labelId, ct) ?? throw new KeyNotFoundException($"Label {labelId} not found.");

        label.PrintCount++;
        label.PrintEvents.Add(new LabelPrintEvent
        {
            LabelId = label.Id,
            IsReprint = true,
            Reason = request.Reason,
            PrinterId = request.PrinterId,
            PrintedByUserId = request.PrintedByUserId
        });

        await _labels.UpdateAsync(label, ct);
        return ToResponse(label);
    }

    private static LabelResponse ToResponse(Label label) => new(
        label.Id, label.Type, label.Format, label.BarcodeValue, label.RenderedContentUri, label.PrintCount);
}
