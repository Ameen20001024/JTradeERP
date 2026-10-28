using FluentValidation;

namespace JTradeERP.Application.Features.Billing.Invoices.UpdateInvoice;

public sealed class UpdateInvoiceValidator:AbstractValidator<UpdateInvoiceCommand>
{
    public UpdateInvoiceValidator()
    {
        RuleFor(x=>x.Id).NotEmpty();
        RuleFor(x=>x.CustomerId).NotEmpty();
        RuleFor(x=>x.WarehouseId).NotEmpty();
        RuleFor(x=>x.Lines).NotEmpty();
        RuleForEach(x=>x.Lines).ChildRules(l=>
        {
            l.RuleFor(x=>x.ProductId).NotEmpty();
            l.RuleFor(x=>x.Quantity).GreaterThan(0);
            l.RuleFor(x=>x.UnitPrice).GreaterThanOrEqualTo(0);
            l.RuleFor(x=>x.TaxCodeId).NotEmpty();
        });
    }
}
