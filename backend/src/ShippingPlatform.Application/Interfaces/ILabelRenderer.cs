using ShippingPlatform.Domain.Enums;

namespace ShippingPlatform.Application.Interfaces;

/// <summary>Renders label content for a given type/format from template + shipment/kit metadata (BR-020–022).</summary>
public interface ILabelRenderer
{
    Task<RenderedLabel> RenderAsync(LabelRenderContext context, CancellationToken ct = default);
}

public record LabelRenderContext(
    LabelType Type,
    LabelFormat Format,
    string TemplateKey,
    IReadOnlyDictionary<string, string> DataFields);

public record RenderedLabel(byte[] Content, string BarcodeValue, string ContentType);

/// <summary>Routes a rendered label to the correct physical printer for the lab/warehouse/kit type (BR-024).</summary>
public interface IPrintRoutingService
{
    Task<string> ResolvePrinterIdAsync(string locationCode, LabelType labelType, CancellationToken ct = default);
    Task SendToPrinterAsync(string printerId, RenderedLabel label, CancellationToken ct = default);
}
