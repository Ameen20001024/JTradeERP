namespace JTradeERP.Application.Common.Inventory;

public sealed record StockBalanceItemResponse(Guid ProductId, string ProductCode, string ProductName,
    Guid WarehouseId, string WarehouseCode, string WarehouseName, decimal QtyOnHand, decimal QtyReserved,
    decimal AvailableQuantity, decimal WeightedAverageCost, string RowVersion);

public sealed record StockLedgerItemResponse(Guid Id, Guid ProductId, Guid WarehouseId, string MovementType,
    string Direction, decimal Quantity, decimal UnitCost, string SourceDocumentType, Guid SourceDocumentId,
    Guid? SourceLineId, Guid? ReversalOfLedgerEntryId, DateTimeOffset PostedAt, Guid PostedByUserId, string CorrelationId);

public sealed record ReorderAlertItemResponse(Guid ProductId, string ProductCode, string ProductName,
    Guid WarehouseId, string WarehouseCode, string WarehouseName, decimal QtyOnHand, decimal QtyReserved,
    decimal AvailableQuantity, decimal MinimumStockLevel, decimal ReorderQuantity);
