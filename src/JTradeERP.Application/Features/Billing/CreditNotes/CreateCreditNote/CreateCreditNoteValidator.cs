using FluentValidation;

namespace JTradeERP.Application.Features.Billing.CreditNotes.CreateCreditNote;

public sealed class CreateCreditNoteValidator:AbstractValidator<CreateCreditNoteCommand>
{
    public CreateCreditNoteValidator()
    {
        RuleFor(x=>x.InvoiceId).NotEmpty();
        RuleFor(x=>x.Lines).NotEmpty();
        RuleForEach(x=>x.Lines).ChildRules(l=>
            {
                l.RuleFor(x => x.InvoiceLineId).NotEmpty();
                l.RuleFor(x=>x.Quantity).GreaterThan(0);
            });
        
        RuleFor(x=>x.Lines)
            .Must(x => x.Select(l => l.InvoiceLineId)
            .Distinct()
            .Count()==x.Count)
            .WithMessage("An invoice line can only be listed once per credit note.");
    }
}
