using FluentValidation;

namespace JTradeERP.Application.Features.Billing.SalesOrders.CreateSalesOrder;

public sealed class CreateSalesOrderValidator:AbstractValidator<CreateSalesOrderCommand>
{
    public CreateSalesOrderValidator()
    {
        RuleFor(x=>x.CustomerId).NotEmpty();
        RuleFor(x=>x.WarehouseId).NotEmpty();
        RuleFor(x=>x.DeliveryDate).Must((x,d)=>d==null||d>=x.OrderDate);
        RuleFor(x=>x.Lines).NotEmpty();
        RuleForEach(x=>x.Lines).ChildRules(l=>
        {
            l.RuleFor(x=>x.ProductId).NotEmpty();
            l.RuleFor(x=>x.OrderedQuantity).GreaterThan(0);
            l.RuleFor(x=>x.UnitPrice).GreaterThanOrEqualTo(0);
            l.RuleFor(x=>x.DiscountAmount).GreaterThanOrEqualTo(0);
            l.RuleFor(x=>x.TaxCodeId).NotEmpty();
        });
    }
}
