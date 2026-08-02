-- GoLIMS Shipping Platform — Core schema (SQL Server)
-- Matches the EF Core model in backend/src/ShippingPlatform.Infrastructure/Persistence.
-- Run in order: 001_init_schema.sql -> 002_seed_data.sql -> 003_custody_immutability.sql

IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = 'ShippingPlatform')
BEGIN
    CREATE DATABASE ShippingPlatform;
END
GO

USE ShippingPlatform;
GO

-- ============================================================================
-- Carriers (E2 — Carrier Integration Layer)
-- ============================================================================
CREATE TABLE Carriers (
    Id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Code            NVARCHAR(20)  NOT NULL UNIQUE,
    Name            NVARCHAR(100) NOT NULL,
    AdapterKey      NVARCHAR(50)  NOT NULL,
    IsActive        BIT           NOT NULL DEFAULT 1,
    SupportsEdi     BIT           NOT NULL DEFAULT 0,
    Priority        INT           NOT NULL DEFAULT 100
);

CREATE TABLE CarrierServices (
    Id                          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CarrierId                   UNIQUEIDENTIFIER NOT NULL REFERENCES Carriers(Id) ON DELETE CASCADE,
    ServiceCode                 NVARCHAR(30)  NOT NULL,
    ServiceName                 NVARCHAR(100) NOT NULL,
    SupportsTemperatureControl  BIT NOT NULL DEFAULT 0,
    SupportsBiologicalMaterial  BIT NOT NULL DEFAULT 0
);
CREATE INDEX IX_CarrierServices_CarrierId ON CarrierServices(CarrierId);

-- ============================================================================
-- Kits & collection logistics (E4 — the capability D365 rates lowest)
-- ============================================================================
CREATE TABLE Kits (
    Id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    KitNumber           NVARCHAR(30) NOT NULL UNIQUE,
    KitTypeCode         NVARCHAR(50) NOT NULL,
    Status              NVARCHAR(20) NOT NULL DEFAULT 'Created',
    D365OrderReference  NVARCHAR(50) NULL,
    CreatedAtUtc        DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET()
);

CREATE TABLE KitComponents (
    Id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    KitId           UNIQUEIDENTIFIER NOT NULL REFERENCES Kits(Id) ON DELETE CASCADE,
    ComponentCode   NVARCHAR(30)  NOT NULL,
    ComponentName   NVARCHAR(100) NOT NULL,
    Quantity        INT NOT NULL DEFAULT 1,
    IsRequired      BIT NOT NULL DEFAULT 1,
    IsFulfilled     BIT NOT NULL DEFAULT 0,
    ConsumableSku   NVARCHAR(30) NULL
);
CREATE INDEX IX_KitComponents_KitId ON KitComponents(KitId);

-- Append-only chain-of-custody trail (BR-033). See 003_custody_immutability.sql for the
-- trigger that enforces no UPDATE/DELETE at the database layer.
CREATE TABLE ChainOfCustodyEvents (
    Id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    KitId           UNIQUEIDENTIFIER NOT NULL REFERENCES Kits(Id) ON DELETE CASCADE,
    ShipmentId      UNIQUEIDENTIFIER NULL,
    Action          NVARCHAR(20) NOT NULL,
    Location        NVARCHAR(100) NULL,
    ActorUserId     NVARCHAR(100) NULL,
    OccurredAtUtc   DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    Notes           NVARCHAR(500) NULL
);
CREATE INDEX IX_ChainOfCustodyEvents_KitId ON ChainOfCustodyEvents(KitId);

-- ============================================================================
-- Shipments & packages (E1 — Shipment Orchestration Engine)
-- ============================================================================
CREATE TABLE Shipments (
    Id                          UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    D365OrderReference          NVARCHAR(50) NULL,
    ShipmentNumber              NVARCHAR(30) NOT NULL UNIQUE,
    Direction                   NVARCHAR(20) NOT NULL,
    Status                      NVARCHAR(20) NOT NULL DEFAULT 'Created',
    MaterialClassification      NVARCHAR(30) NOT NULL DEFAULT 'Standard',
    OriginCountryCode           NCHAR(2) NOT NULL,
    DestinationCountryCode      NCHAR(2) NOT NULL,
    OriginAddress               NVARCHAR(300) NOT NULL,
    DestinationAddress          NVARCHAR(300) NOT NULL,
    CarrierId                   UNIQUEIDENTIFIER NULL REFERENCES Carriers(Id),
    CarrierServiceCode          NVARCHAR(30) NULL,
    CarrierTrackingNumber       NVARCHAR(60) NULL,
    ConsolidatedLoadId          UNIQUEIDENTIFIER NULL,
    KitId                       UNIQUEIDENTIFIER NULL REFERENCES Kits(Id),
    FreightCost                 DECIMAL(18,2) NULL,
    CurrencyCode                NCHAR(3) NOT NULL DEFAULT 'USD',
    RequiresTemperatureControl  BIT NOT NULL DEFAULT 0,
    IsVipHandling               BIT NOT NULL DEFAULT 0,
    CustomerType                NVARCHAR(50) NULL,
    CreatedAtUtc                DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    DispatchedAtUtc             DATETIMEOFFSET NULL,
    DeliveredAtUtc              DATETIMEOFFSET NULL
);
CREATE INDEX IX_Shipments_Status ON Shipments(Status);
CREATE INDEX IX_Shipments_D365OrderReference ON Shipments(D365OrderReference);

CREATE TABLE ShipmentStatusHistories (
    Id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ShipmentId      UNIQUEIDENTIFIER NOT NULL REFERENCES Shipments(Id) ON DELETE CASCADE,
    Status          NVARCHAR(20) NOT NULL,
    OccurredAtUtc   DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    ChangedByUserId NVARCHAR(100) NULL,
    Notes           NVARCHAR(500) NULL
);
CREATE INDEX IX_ShipmentStatusHistories_ShipmentId ON ShipmentStatusHistories(ShipmentId);

CREATE TABLE Packages (
    Id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ShipmentId      UNIQUEIDENTIFIER NOT NULL REFERENCES Shipments(Id) ON DELETE CASCADE,
    PackageNumber   NVARCHAR(30) NOT NULL,
    WeightKg        DECIMAL(10,3) NOT NULL DEFAULT 0,
    LengthCm        DECIMAL(10,2) NOT NULL DEFAULT 0,
    WidthCm         DECIMAL(10,2) NOT NULL DEFAULT 0,
    HeightCm        DECIMAL(10,2) NOT NULL DEFAULT 0,
    LicensePlate    NVARCHAR(30) NULL
);
CREATE INDEX IX_Packages_ShipmentId ON Packages(ShipmentId);

-- ============================================================================
-- Labels (E3 — Label Management Service)
-- ============================================================================
CREATE TABLE Labels (
    Id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PackageId           UNIQUEIDENTIFIER NOT NULL REFERENCES Packages(Id) ON DELETE CASCADE,
    Type                NVARCHAR(20) NOT NULL,
    Format              NVARCHAR(10) NOT NULL,
    TemplateKey         NVARCHAR(100) NOT NULL,
    RenderedContentUri  NVARCHAR(500) NULL,
    BarcodeValue        NVARCHAR(60) NOT NULL,
    PrintCount          INT NOT NULL DEFAULT 0,
    CreatedAtUtc        DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET()
);
CREATE INDEX IX_Labels_PackageId ON Labels(PackageId);

CREATE TABLE LabelPrintEvents (
    Id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    LabelId         UNIQUEIDENTIFIER NOT NULL REFERENCES Labels(Id) ON DELETE CASCADE,
    IsReprint       BIT NOT NULL DEFAULT 0,
    Reason          NVARCHAR(300) NULL,
    PrintedByUserId NVARCHAR(100) NULL,
    PrinterId       NVARCHAR(50) NULL,
    OccurredAtUtc   DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET()
);
CREATE INDEX IX_LabelPrintEvents_LabelId ON LabelPrintEvents(LabelId);

-- ============================================================================
-- Rules Engine (E5) & D365 Sync Outbox (E7)
-- ============================================================================
CREATE TABLE RuleDefinitions (
    Id                      UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name                    NVARCHAR(150) NOT NULL,
    Category                NVARCHAR(50)  NOT NULL,
    Priority                INT NOT NULL DEFAULT 0,
    IsActive                BIT NOT NULL DEFAULT 1,
    ConditionExpression     NVARCHAR(1000) NOT NULL DEFAULT '',
    ActionExpression        NVARCHAR(500)  NOT NULL DEFAULT '',
    ApplicableCountryCode   NCHAR(2) NULL,
    CreatedAtUtc            DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    UpdatedAtUtc            DATETIMEOFFSET NULL
);

CREATE TABLE D365SyncRecords (
    Id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Direction       NVARCHAR(10) NOT NULL,
    EntityType      NVARCHAR(30) NOT NULL,
    Status          NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    ShipmentId      UNIQUEIDENTIFIER NULL REFERENCES Shipments(Id),
    KitId           UNIQUEIDENTIFIER NULL REFERENCES Kits(Id),
    Payload         NVARCHAR(MAX) NOT NULL,
    AttemptCount    INT NOT NULL DEFAULT 0,
    LastError       NVARCHAR(1000) NULL,
    CreatedAtUtc    DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    LastAttemptAtUtc DATETIMEOFFSET NULL,
    CompletedAtUtc  DATETIMEOFFSET NULL
);
CREATE INDEX IX_D365SyncRecords_Status ON D365SyncRecords(Status);
GO
