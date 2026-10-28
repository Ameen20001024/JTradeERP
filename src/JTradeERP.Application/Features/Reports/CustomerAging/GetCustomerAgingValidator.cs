using FluentValidation;

namespace JTradeERP.Application.Features.Reports.CustomerAging;

public sealed class GetCustomerAgingValidator : AbstractValidator<GetCustomerAgingQuery>
{
    private static readonly string[] SortFields = ["customer", "current", "days1to30", "days31to60", "days61to90", "days91plus", "totaloutstanding"];

    public GetCustomerAgingValidator()
    {
        RuleFor(x => x.AsOfDate).Must(x => x > DateOnly.MinValue)
            .WithMessage("AsOfDate is required.");
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 500);
        RuleFor(x => x.SortBy).Must(x => SortFields.Contains(x, StringComparer.OrdinalIgnoreCase));
        RuleFor(x => x.SortDirection).Must(x => x is "asc" or "desc" || x is "ASC" or "DESC");
    }
}
