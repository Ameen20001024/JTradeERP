using FluentValidation;

namespace JTradeERP.Application.Features.Billing.CreditNotes.UpdateCreditNote;

public sealed class UpdateCreditNoteValidator:AbstractValidator<UpdateCreditNoteCommand>
{
    public UpdateCreditNoteValidator()
    {
        RuleFor(x=>x.Id).NotEmpty();
        RuleFor(x=>x.Lines).NotEmpty();
        RuleForEach(x=>x.Lines).ChildRules(l=>
        {
            l.RuleFor(x=>x.InvoiceLineId).NotEmpty();
            l.RuleFor(x=>x.Quantity).GreaterThan(0);
        });
    }
}
