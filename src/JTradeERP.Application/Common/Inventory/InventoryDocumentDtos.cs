namespace JTradeERP.Application.Common.Inventory;
public sealed record StockAdjustmentItemResponse(Guid Id,Guid ProductId,Guid WarehouseId,decimal QuantityDelta,decimal? UnitCost,
    string ReasonCode,string Status,DateTimeOffset? PostedAt,DateTimeOffset? ReversedAt);
public sealed record StockTransferLineItemResponse(Guid Id,Guid ProductId,decimal Quantity);
public sealed record StockTransferItemResponse(Guid Id,Guid SourceWarehouseId,Guid DestinationWarehouseId,string Status,
    DateTimeOffset? PostedAt,IReadOnlyCollection<StockTransferLineItemResponse> Lines);
