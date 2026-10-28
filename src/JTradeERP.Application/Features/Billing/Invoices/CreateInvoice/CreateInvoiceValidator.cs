using FluentValidation;

namespace JTradeERP.Application.Features.Billing.Invoices.CreateInvoice;

public sealed class CreateInvoiceValidator:AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceValidator()
    {
        RuleFor(x=>x.CustomerId).NotEmpty();
        RuleFor(x=>x.WarehouseId).NotEmpty();
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
