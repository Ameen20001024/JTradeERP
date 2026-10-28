using FluentValidation;
namespace JTradeERP.Application.Features.Billing.SalesOrders.UpdateSalesOrder;

public sealed class UpdateSalesOrderValidator:AbstractValidator<UpdateSalesOrderCommand>
{
    public UpdateSalesOrderValidator()
    {
        RuleFor(x=>x.Id).NotEmpty();
        RuleFor(x=>x.CustomerId).NotEmpty();
        RuleFor(x=>x.WarehouseId).NotEmpty();
        RuleFor(x=>x.Lines).NotEmpty();
        RuleForEach(x=>x.Lines).ChildRules(l=>
        {
            l.RuleFor(x=>x.ProductId).NotEmpty();
            l.RuleFor(x=>x.OrderedQuantity).GreaterThan(0);
            l.RuleFor(x=>x.UnitPrice).GreaterThanOrEqualTo(0);
            l.RuleFor(x=>x.TaxCodeId).NotEmpty();
        });
    }
}
