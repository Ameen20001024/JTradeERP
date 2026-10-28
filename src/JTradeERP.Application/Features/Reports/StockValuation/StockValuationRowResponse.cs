namespace JTradeERP.Application.Features.Reports.StockValuation;

public sealed record StockValuationRowResponse(Guid WarehouseId, string WarehouseCode, string WarehouseName,
    Guid ProductId, string ProductCode, string ProductName, Guid CategoryId, string CategoryName,
    decimal QtyOnHand, decimal WeightedAverageCost, decimal InventoryValue);
