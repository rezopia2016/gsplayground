-- Seed data: reference carriers and sample business rules for the GoLIMS Shipping Platform.
USE ShippingPlatform;
GO

INSERT INTO Carriers (Code, Name, AdapterKey, IsActive, SupportsEdi, Priority) VALUES
    ('DHL',   'DHL Express',          'dhl',   1, 1, 10),
    ('UPS',   'United Parcel Service','ups',   1, 1, 20),
    ('NITSU', 'Nitsu Regional Courier','nitsu',1, 1, 30);
GO

INSERT INTO CarrierServices (CarrierId, ServiceCode, ServiceName, SupportsTemperatureControl, SupportsBiologicalMaterial)
SELECT Id, 'EXPRESS', 'DHL Express Worldwide', 1, 1 FROM Carriers WHERE Code = 'DHL';
INSERT INTO CarrierServices (CarrierId, ServiceCode, ServiceName, SupportsTemperatureControl, SupportsBiologicalMaterial)
SELECT Id, 'ECONOMY', 'DHL Economy Select', 0, 0 FROM Carriers WHERE Code = 'DHL';
INSERT INTO CarrierServices (CarrierId, ServiceCode, ServiceName, SupportsTemperatureControl, SupportsBiologicalMaterial)
SELECT Id, 'WWEF', 'UPS Worldwide Express Freight', 1, 1 FROM Carriers WHERE Code = 'UPS';
INSERT INTO CarrierServices (CarrierId, ServiceCode, ServiceName, SupportsTemperatureControl, SupportsBiologicalMaterial)
SELECT Id, 'STD', 'Nitsu Standard Regional', 1, 1 FROM Carriers WHERE Code = 'NITSU';
GO

-- Sample business rules (E5) — externalized configuration, not hardcoded application logic.
INSERT INTO RuleDefinitions (Name, Category, Priority, ConditionExpression, ActionExpression, ApplicableCountryCode) VALUES
    ('Biological material requires hazard label', 'Compliance', 100,
     'MaterialClassification=BiologicalSample', 'RequireHazardLabel,RequireComplianceDocumentation', NULL),
    ('VIP customer expedited handling', 'CustomerHandling', 50,
     'IsVipHandling=True', 'RequireExpeditedService', NULL),
    ('EU biological shipment compliance documentation', 'Compliance', 90,
     'MaterialClassification=BiologicalSample', 'RequireComplianceDocumentation', 'DE'),
    ('Japan courier compliance documentation', 'Compliance', 90,
     '', 'RequireComplianceDocumentation', 'JP');
GO
