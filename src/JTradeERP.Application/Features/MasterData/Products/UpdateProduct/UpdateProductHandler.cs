using MediatR;
using JTradeERP.Application.Common.Persistence; 
using JTradeERP.Application.Common.Exceptions; 
using JTradeERP.Application.Features.MasterData.Products.CreateProduct;

namespace JTradeERP.Application.Features.MasterData.Products.UpdateProduct;
public sealed class UpdateProductHandler(IApplicationDbContext db) 
    : IRequestHandler<UpdateProductCommand,UpdateProductResponse> 

{ 
    public async Task<UpdateProductResponse> Handle(UpdateProductCommand r,CancellationToken ct) 
    {
        var e = db.Products.SingleOrDefault( x=> x.Id == r.Id)
            ??throw new NotFoundException("Product not found.");
        
        if(db.Products.Any(x => x.Id != r.Id && x.ProductCode == r.ProductCode.Trim()))
            throw new ConflictException("Product code already exists.");
        
        CreateProductHandler.EnsureReferences(db,r.CategoryId,r.StockUomId,r.TaxCodeId);
        
        e.Update(
            r.ProductCode,
            r.Name,
            r.CategoryId,
            r.StockUomId,
            r.TaxCodeId,
            r.MinimumStockLevel,
            r.ReorderQuantity,
            r.SalesPrice,
            r.PurchasePrice);
        
        await db.SaveChangesAsync(ct);
        
        return new UpdateProductResponse(
            e.Id,
            e.ProductCode,
            e.Name,
            e.CategoryId,
            e.StockUomId,
            e.TaxCodeId,
            e.MinimumStockLevel,
            e.ReorderQuantity,
            e.SalesPrice,
            e.PurchasePrice,
            e.IsActive);
    } 
}
