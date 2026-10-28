using FluentValidation;

namespace JTradeERP.Application.Features.Billing.Invoices.GetInvoices;

public sealed class GetInvoicesValidator:AbstractValidator<GetInvoicesQuery>
{
    public GetInvoicesValidator()
    {
        RuleFor(x=>x.Page).GreaterThan(0);
        RuleFor(x=>x.PageSize).InclusiveBetween(1,200);
        RuleFor(x=>x.To).Must((x,to)=>to==null||x.From==null||to>=x.From);
    }
}
