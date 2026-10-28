using FluentValidation;

namespace JTradeERP.Application.Features.Billing.Payments.RecordPayment;

public sealed class RecordPaymentValidator:AbstractValidator<RecordPaymentCommand>
{
    public RecordPaymentValidator()
    {
        RuleFor(x=>x.PaymentMethodId).NotEmpty();
        RuleFor(x=>x.Amount).GreaterThan(0);
        RuleFor(x=>x.Allocations).NotEmpty();
        RuleForEach(x=>x.Allocations).ChildRules(a=>
        {
            a.RuleFor(x=>x.InvoiceId).NotEmpty();
            a.RuleFor(x=>x.Amount).GreaterThan(0);
        });
        
        RuleFor(x=>x.Allocations).Must(x=>x.Select(a=>a.InvoiceId).Distinct().Count()==x.Count).WithMessage("Each invoice may be allocated once per payment.");
        
        RuleFor(x=>x.Allocations).Must((p,a)=>a.Sum(x=>x.Amount)<=p.Amount).WithMessage("Payment allocations cannot exceed payment amount.");
    }
}
