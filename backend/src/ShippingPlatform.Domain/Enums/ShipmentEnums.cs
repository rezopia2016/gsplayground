namespace ShippingPlatform.Domain.Enums;

public enum ShipmentStatus
{
    Created,
    Ready,
    Dispatched,
    InTransit,
    Delivered,
    Exception,
    Cancelled
}

public enum ShipmentDirection
{
    Outbound,
    Inbound,
    Return
}

public enum MaterialClassification
{
    Standard,
    BiologicalSample,
    DryIce,
    HazardousMaterial,
    TemperatureControlled
}

public enum KitStatus
{
    Created,
    Assembling,
    ReadyForDispatch,
    Dispatched,
    Collected,
    InTransit,
    Returned,
    Received,
    Incomplete
}

public enum LabelType
{
    Customer,
    Internal,
    Sample,
    CollectionKit,
    Hazard,
    Return
}

public enum LabelFormat
{
    Zpl,
    Pdf,
    Qr
}

public enum CustodyAction
{
    Created,
    Packed,
    Dispatched,
    PickedUp,
    Transferred,
    Received,
    Returned
}

public enum D365SyncDirection
{
    Inbound,
    Outbound
}

public enum D365SyncStatus
{
    Pending,
    Synced,
    Failed,
    Retrying
}

public enum D365SyncEntityType
{
    Order,
    CustomerAddress,
    BillingEvent,
    InventoryEvent,
    ComplianceDocument
}
