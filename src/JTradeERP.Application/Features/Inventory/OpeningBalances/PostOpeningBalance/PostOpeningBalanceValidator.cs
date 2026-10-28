using FluentValidation;
using JTradeERP.Application.Common.Inventory;

namespace JTradeERP.Application.Features.Inventory.OpeningBalances.PostOpeningBalance;

public sealed class PostOpeningBalanceValidator : AbstractValidator<PostOpeningBalanceCommand>
{
    public PostOpeningBalanceValidator()
    {
        RuleFor(x => x.SourceDocumentId).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.WarehouseId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0).Must(InventoryPrecision.IsQuantity);
        RuleFor(x => x.UnitCost).Must(InventoryPrecision.IsUnitCost);
    }
}
