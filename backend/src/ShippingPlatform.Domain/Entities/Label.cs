using ShippingPlatform.Domain.Enums;

namespace ShippingPlatform.Domain.Entities;

public class Label
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PackageId { get; set; }
    public LabelType Type { get; set; }
    public LabelFormat Format { get; set; }
    public string TemplateKey { get; set; } = string.Empty;
    public string? RenderedContentUri { get; set; }
    public string BarcodeValue { get; set; } = string.Empty;
    public int PrintCount { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public List<LabelPrintEvent> PrintEvents { get; set; } = new();
}

/// <summary>Audit trail entry for every print/reprint action (BR-023 / TC-E3-05).</summary>
public class LabelPrintEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LabelId { get; set; }
    public bool IsReprint { get; set; }
    public string? Reason { get; set; }
    public string? PrintedByUserId { get; set; }
    public string? PrinterId { get; set; }
    public DateTimeOffset OccurredAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
