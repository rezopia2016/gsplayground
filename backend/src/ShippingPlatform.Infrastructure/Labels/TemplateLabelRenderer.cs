using System.Text;
using ShippingPlatform.Application.Interfaces;
using ShippingPlatform.Domain.Enums;

namespace ShippingPlatform.Infrastructure.Labels;

/// <summary>
/// Template-driven label renderer covering ZPL, PDF, and QR output (BR-021/022) — the capability
/// the assessment rates D365 lowest on ("Primarily ZPL; enterprise print routing needs
/// middleware"). Real PDF rendering should use a library such as QuestPDF; ZPL is generated
/// directly since it is a text protocol. This implementation demonstrates the template contract;
/// swap in a real rendering engine per format as templates are finalized with lab operations.
/// </summary>
public class TemplateLabelRenderer : ILabelRenderer
{
    public Task<RenderedLabel> RenderAsync(LabelRenderContext context, CancellationToken ct = default)
    {
        var barcodeValue = $"{context.Type}-{Guid.NewGuid():N}"[..24].ToUpperInvariant();

        var content = context.Format switch
        {
            LabelFormat.Zpl => Encoding.UTF8.GetBytes(BuildZpl(context, barcodeValue)),
            LabelFormat.Pdf => Encoding.UTF8.GetBytes(BuildPdfPlaceholder(context, barcodeValue)),
            LabelFormat.Qr => Encoding.UTF8.GetBytes(barcodeValue),
            _ => throw new NotSupportedException($"Label format {context.Format} is not supported.")
        };

        var contentType = context.Format switch
        {
            LabelFormat.Zpl => "text/plain",
            LabelFormat.Pdf => "application/pdf",
            LabelFormat.Qr => "image/png",
            _ => "application/octet-stream"
        };

        return Task.FromResult(new RenderedLabel(content, barcodeValue, contentType));
    }

    private static string BuildZpl(LabelRenderContext context, string barcodeValue)
    {
        var fields = string.Join(Environment.NewLine, context.DataFields.Select(f => $"^FD{f.Key}: {f.Value}^FS"));
        return $"^XA^FO50,50^BY3^BCN,100,Y,N,N^FD{barcodeValue}^FS{Environment.NewLine}{fields}^XZ";
    }

    private static string BuildPdfPlaceholder(LabelRenderContext context, string barcodeValue) =>
        $"[PDF label placeholder] Type={context.Type} Template={context.TemplateKey} Barcode={barcodeValue} " +
        string.Join(' ', context.DataFields.Select(f => $"{f.Key}={f.Value}"));
}
