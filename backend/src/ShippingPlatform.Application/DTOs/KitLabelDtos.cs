using ShippingPlatform.Domain.Enums;

namespace ShippingPlatform.Application.DTOs;

public record KitComponentRequest(string ComponentCode, string ComponentName, int Quantity, bool IsRequired, string? ConsumableSku);

public record CreateKitRequest(string KitTypeCode, string? D365OrderReference, List<KitComponentRequest> Components);

public record KitResponse(
    Guid Id,
    string KitNumber,
    string KitTypeCode,
    KitStatus Status,
    bool IsComplete,
    List<KitComponentResponse> Components);

public record KitComponentResponse(string ComponentCode, string ComponentName, int Quantity, bool IsRequired, bool IsFulfilled);

public record RecordCustodyEventRequest(CustodyAction Action, string? Location, string? ActorUserId, string? Notes, Guid? ShipmentId);

public record GenerateLabelsRequest(Guid PackageId, List<LabelType> RequestedTypes, LabelFormat Format);

public record LabelResponse(Guid Id, LabelType Type, LabelFormat Format, string BarcodeValue, string? RenderedContentUri, int PrintCount);

public record ReprintLabelRequest(string Reason, string? PrinterId, string? PrintedByUserId);
