namespace JTradeERP.Application.Features.Reports.LowStock;

public sealed record LowStockRowResponse(Guid WarehouseId, string WarehouseCode, string WarehouseName,
    Guid ProductId, string ProductCode, string ProductName, Guid CategoryId, string CategoryName,
    decimal QtyOnHand, decimal QtyReserved, decimal AvailableQuantity, decimal MinimumStockLevel, decimal ReorderQuantity);
