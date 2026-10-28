using FluentValidation;

namespace JTradeERP.Application.Features.Billing.CreditNotes.GetCreditNotes;

public sealed class GetCreditNotesValidator:AbstractValidator<GetCreditNotesQuery>
{
    public GetCreditNotesValidator()
    {
        RuleFor(x=>x.Page).GreaterThan(0);
        RuleFor(x=>x.PageSize).InclusiveBetween(1,200);
    }
}
