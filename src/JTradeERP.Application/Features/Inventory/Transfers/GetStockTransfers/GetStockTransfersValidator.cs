using FluentValidation;
using JTradeERP.Domain.Inventory;
namespace JTradeERP.Application.Features.Inventory.Transfers.GetStockTransfers;
public sealed class GetStockTransfersValidator : AbstractValidator<GetStockTransfersQuery>
{
    public GetStockTransfersValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
        RuleFor(x => x.Status).Must(x => string.IsNullOrWhiteSpace(x) || Enum.TryParse<InventoryDocumentStatus>(x, true, out _));
    }
}
