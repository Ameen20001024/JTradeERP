using FluentValidation;

namespace JTradeERP.Application.Features.Reports.StockMovement;

public sealed class GetStockMovementValidator : AbstractValidator<GetStockMovementQuery>
{
    private static readonly string[] SortFields = ["postedAt", "warehouse", "product", "movementType", "sourceDocumentType"];
    public GetStockMovementValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 500);
        RuleFor(x => x.ToUtc).Must((q, to) => !q.FromUtc.HasValue || !to.HasValue || to >= q.FromUtc).WithMessage("ToUtc must not precede FromUtc.");
        RuleFor(x => x.SortBy).Must(x => SortFields.Contains(x, StringComparer.OrdinalIgnoreCase));
        RuleFor(x => x.SortDirection).Must(x => x is "asc" or "desc" || x is "ASC" or "DESC");
        RuleFor(x => x.SourceDocumentType).MaximumLength(64);
    }
}
