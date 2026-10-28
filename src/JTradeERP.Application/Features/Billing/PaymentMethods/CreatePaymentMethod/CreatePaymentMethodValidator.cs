using FluentValidation;

namespace JTradeERP.Application.Features.Billing.PaymentMethods.CreatePaymentMethod;

public sealed class CreatePaymentMethodValidator:AbstractValidator<CreatePaymentMethodCommand>
{
    public CreatePaymentMethodValidator()
    {
        RuleFor(x=>x.Code)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(x=>x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}
