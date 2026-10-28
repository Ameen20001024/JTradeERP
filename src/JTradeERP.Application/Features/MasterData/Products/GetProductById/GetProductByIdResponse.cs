namespace JTradeERP.Application.Features.MasterData.Products.GetProductById;
public sealed record GetProductByIdResponse(Guid Id, string ProductCode, string Name, Guid CategoryId, Guid StockUomId, Guid TaxCodeId, decimal MinimumStockLevel, decimal ReorderQuantity, decimal SalesPrice, decimal PurchasePrice, bool IsActive);
