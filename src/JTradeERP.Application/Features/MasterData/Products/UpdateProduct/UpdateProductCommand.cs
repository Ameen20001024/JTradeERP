using MediatR;
namespace JTradeERP.Application.Features.MasterData.Products.UpdateProduct;
public sealed record UpdateProductCommand(
    Guid Id, 
    string ProductCode, 
    string Name, 
    Guid CategoryId, 
    Guid StockUomId, 
    Guid TaxCodeId, 
    decimal MinimumStockLevel, 
    decimal ReorderQuantity, 
    decimal SalesPrice, 
    decimal PurchasePrice) : IRequest<UpdateProductResponse>;
