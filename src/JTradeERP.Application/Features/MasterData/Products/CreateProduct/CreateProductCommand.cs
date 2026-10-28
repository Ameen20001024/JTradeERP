using MediatR;
namespace JTradeERP.Application.Features.MasterData.Products.CreateProduct;
public sealed record CreateProductCommand(
    string ProductCode, 
    string Name, 
    Guid CategoryId, 
    Guid StockUomId, 
    Guid TaxCodeId, 
    decimal MinimumStockLevel, 
    decimal ReorderQuantity, 
    decimal SalesPrice, 
    decimal PurchasePrice) : IRequest<CreateProductResponse>;
