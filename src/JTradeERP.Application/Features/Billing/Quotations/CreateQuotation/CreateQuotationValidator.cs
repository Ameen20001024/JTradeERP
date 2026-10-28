using FluentValidation;
namespace JTradeERP.Application.Features.Billing.Quotations.CreateQuotation;
public sealed class CreateQuotationValidator : AbstractValidator<CreateQuotationCommand>
{
    public CreateQuotationValidator()
    {
        RuleFor(x=>x.CustomerId).NotEmpty();
        RuleFor(x=>x.ValidityDate).GreaterThanOrEqualTo(x=>x.QuotationDate);
        RuleFor(x=>x.Lines).NotEmpty();
        RuleForEach(x=>x.Lines).ChildRules(l=>
        {
            l.RuleFor(x=>x.ProductId).NotEmpty();
            l.RuleFor(x=>x.Quantity).GreaterThan(0);
            l.RuleFor(x=>x.UnitPrice).GreaterThanOrEqualTo(0);
            l.RuleFor(x=>x.DiscountAmount).GreaterThanOrEqualTo(0);
            l.RuleFor(x=>x.TaxCodeId).NotEmpty();
        });
    }
}
