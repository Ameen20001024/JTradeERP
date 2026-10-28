using FluentValidation;

namespace JTradeERP.Application.Features.Reports.LowStock;

public sealed class GetLowStockValidator : AbstractValidator<GetLowStockQuery>
{
    private static readonly string[] SortFields = ["warehouse", "product", "available", "minimum", "reorder"];
    public GetLowStockValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 500);
        RuleFor(x => x.SortBy).Must(x => SortFields.Contains(x, StringComparer.OrdinalIgnoreCase));
        RuleFor(x => x.SortDirection).Must(x => x is "asc" or "desc" || x is "ASC" or "DESC");
    }
}
