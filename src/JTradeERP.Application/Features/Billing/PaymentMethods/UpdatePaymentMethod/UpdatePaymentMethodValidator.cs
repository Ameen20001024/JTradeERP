using FluentValidation;

namespace JTradeERP.Application.Features.Billing.PaymentMethods.UpdatePaymentMethod;

public sealed class UpdatePaymentMethodValidator:AbstractValidator<UpdatePaymentMethodCommand>
{
    public UpdatePaymentMethodValidator()
    {
        RuleFor(x=>x.Id).NotEmpty();
        
        RuleFor(x=>x.Code).NotEmpty().MaximumLength(30);
        
        RuleFor(x=>x.Name).NotEmpty().MaximumLength(100);
    }
}
