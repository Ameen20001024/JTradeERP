using FluentValidation;

namespace JTradeERP.Application.Features.Billing.NumberSeries.CreateNumberSeries;

public sealed class CreateNumberSeriesValidator:AbstractValidator<CreateNumberSeriesCommand>
{
    public CreateNumberSeriesValidator()
    {
        RuleFor(x=>x.DocumentType).NotEmpty().MaximumLength(50);
        RuleFor(x=>x.Prefix).MaximumLength(30);
        RuleFor(x=>x.NextNumber).GreaterThan(0);
        RuleFor(x=>x.Padding).InclusiveBetween(1,18);
    }
}
