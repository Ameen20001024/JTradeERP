using MediatR; 
using JTradeERP.Application.Common.Persistence; 
using JTradeERP.Application.Common.Exceptions; 
using JTradeERP.Domain.MasterData;

namespace JTradeERP.Application.Features.MasterData.Products.CreateProduct;
public sealed class CreateProductHandler(IApplicationDbContext db) 
    : IRequestHandler<CreateProductCommand,CreateProductResponse> 

{
    public async Task<CreateProductResponse> Handle(CreateProductCommand r,CancellationToken ct) 
    {
        if(db.Products.Any(x => x.ProductCode == r.ProductCode.Trim()))
            throw new ConflictException("Product code already exists."); 
        EnsureReferences(
            db,
            r.CategoryId,
            r.StockUomId,
            r.TaxCodeId);
        
        var e = new Product(
            r.ProductCode,
            r.Name,
            r.CategoryId,
            r.StockUomId,
            r.TaxCodeId,
            r.MinimumStockLevel,
            r.ReorderQuantity,
            r.SalesPrice,
            r.PurchasePrice); 
        
        db.Add(e); 
        
        await db.SaveChangesAsync(ct);

        return Map(e);
    } 
    
    internal static void EnsureReferences(
        IApplicationDbContext db,
        Guid categoryId,
        Guid uomId,
        Guid taxId) 
    {
        if(!db.Categories.Any(x => x.Id == categoryId))
            throw new NotFoundException("Category not found."); 
        
        if(!db.UnitsOfMeasure.Any(x => x.Id == uomId))
            throw new NotFoundException("Unit of Measure not found."); 
        
        if(!db.TaxCodes.Any(x => x.Id == taxId))
            throw new NotFoundException("Tax code not found."); 
    } 
    
    internal static CreateProductResponse Map(Product e) => 
        
        new CreateProductResponse(
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
