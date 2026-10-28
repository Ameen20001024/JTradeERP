namespace JTradeERP.Application.Features.Reports.StockMovement;

public sealed record StockMovementRowResponse(Guid LedgerEntryId, DateTimeOffset PostedAt,
    Guid WarehouseId, string WarehouseCode, string WarehouseName, Guid ProductId, string ProductCode,
    string ProductName, string MovementType, string Direction,
    decimal Quantity, decimal UnitCost, string SourceDocumentType, Guid SourceDocumentId,
    Guid? SourceLineId, Guid? ReversalOfLedgerEntryId, string CorrelationId);
