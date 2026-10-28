using FluentValidation;
namespace JTradeERP.Application.Features.Billing.Quotations.CopyQuotation;

public sealed class CopyQuotationValidator:AbstractValidator<CopyQuotationCommand>
{
    public CopyQuotationValidator()
    {
        RuleFor(x=>x.Id).NotEmpty();
        RuleFor(x=>x.ValidityDate).GreaterThanOrEqualTo(x=>x.QuotationDate);
    }
}
