-- Enforce that chain-of-custody events are append-only at the database layer (BR-033, TC-E4-07).
-- Corrections must be recorded as new events referencing the original event, never as an
-- UPDATE/DELETE of history.
USE ShippingPlatform;
GO

CREATE TRIGGER TR_ChainOfCustodyEvents_PreventUpdate
ON ChainOfCustodyEvents
INSTEAD OF UPDATE
AS
BEGIN
    RAISERROR ('ChainOfCustodyEvents is append-only: updates are not permitted.', 16, 1);
    ROLLBACK TRANSACTION;
END;
GO

CREATE TRIGGER TR_ChainOfCustodyEvents_PreventDelete
ON ChainOfCustodyEvents
INSTEAD OF DELETE
AS
BEGIN
    RAISERROR ('ChainOfCustodyEvents is append-only: deletes are not permitted.', 16, 1);
    ROLLBACK TRANSACTION;
END;
GO

-- Same guarantee for label print history (BR-023) — reprint audit trail must not be alterable.
CREATE TRIGGER TR_LabelPrintEvents_PreventUpdate
ON LabelPrintEvents
INSTEAD OF UPDATE
AS
BEGIN
    RAISERROR ('LabelPrintEvents is append-only: updates are not permitted.', 16, 1);
    ROLLBACK TRANSACTION;
END;
GO

CREATE TRIGGER TR_LabelPrintEvents_PreventDelete
ON LabelPrintEvents
INSTEAD OF DELETE
AS
BEGIN
    RAISERROR ('LabelPrintEvents is append-only: deletes are not permitted.', 16, 1);
    ROLLBACK TRANSACTION;
END;
GO
