using FluentValidation;

namespace JTradeERP.Application.Features.Billing.Configuration.UpdateBillingConfiguration;

public sealed class UpdateBillingConfigurationValidator:AbstractValidator<UpdateBillingConfigurationCommand>
{
    public UpdateBillingConfigurationValidator()
    {
        RuleFor(x=>x.CompanyTrn).MaximumLength(50);
    }
}
