using FluentValidation;

namespace JTradeERP.Application.Features.Billing.SalesOrders.GetSalesOrders;

public sealed class GetSalesOrdersValidator:AbstractValidator<GetSalesOrdersQuery>
{
    public GetSalesOrdersValidator()
    {
        RuleFor(x=>x.Page).GreaterThan(0);
        RuleFor(x=>x.PageSize).InclusiveBetween(1,200);
    }
}
