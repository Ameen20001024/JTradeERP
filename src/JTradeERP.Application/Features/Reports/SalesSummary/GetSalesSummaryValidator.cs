using FluentValidation;

namespace JTradeERP.Application.Features.Reports.SalesSummary;

public sealed class GetSalesSummaryValidator : AbstractValidator<GetSalesSummaryQuery>
{
    private static readonly string[] SortFields = ["date", "customer", "product", "total"];
    public GetSalesSummaryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 500);
        RuleFor(x => x.ToDate).Must((q, to) => !q.FromDate.HasValue || !to.HasValue || to >= q.FromDate).WithMessage("ToDate must not precede FromDate.");
        RuleFor(x => x.SortBy).Must(x => SortFields.Contains(x, StringComparer.OrdinalIgnoreCase));
        RuleFor(x => x.SortDirection).Must(x => x is "asc" or "desc" || x is "ASC" or "DESC");
    }
}
