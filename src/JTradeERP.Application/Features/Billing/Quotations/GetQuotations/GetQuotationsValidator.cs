using FluentValidation;

namespace JTradeERP.Application.Features.Billing.Quotations.GetQuotations;

public sealed class GetQuotationsValidator:AbstractValidator<GetQuotationsQuery>
{
    public GetQuotationsValidator()
    {
        RuleFor(x=>x.Page).GreaterThan(0);
        RuleFor(x=>x.PageSize).InclusiveBetween(1,200);
    }
}
