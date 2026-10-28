using FluentValidation;
namespace JTradeERP.Application.Features.MasterData.Products.CreateProduct;
public sealed class CreateProductValidator : AbstractValidator<CreateProductCommand> 
{
    public CreateProductValidator() 
    { 
        RuleFor(x=>x.ProductCode).NotEmpty().MaximumLength(50); 
        
        RuleFor(x=>x.Name).NotEmpty().MaximumLength(200); 
        
        RuleFor(x=>x.CategoryId).NotEmpty(); 
        
        RuleFor(x=>x.StockUomId).NotEmpty(); 
        
        RuleFor(x=>x.TaxCodeId).NotEmpty(); 
        
        RuleFor(x=>x.MinimumStockLevel).GreaterThanOrEqualTo(0); 
        
        RuleFor(x=>x.ReorderQuantity).GreaterThanOrEqualTo(0); 
        
        RuleFor(x=>x.SalesPrice).GreaterThanOrEqualTo(0); 
        
        RuleFor(x=>x.PurchasePrice).GreaterThanOrEqualTo(0); 
    }
}
