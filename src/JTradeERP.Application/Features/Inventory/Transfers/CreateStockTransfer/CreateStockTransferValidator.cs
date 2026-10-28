using FluentValidation;
using JTradeERP.Application.Common.Inventory;
namespace JTradeERP.Application.Features.Inventory.Transfers.CreateStockTransfer;
public sealed class CreateStockTransferValidator : AbstractValidator<CreateStockTransferCommand>
{
    public CreateStockTransferValidator()
    {
        RuleFor(x => x.SourceWarehouseId).NotEmpty();
        RuleFor(x => x.DestinationWarehouseId).NotEmpty().NotEqual(x => x.SourceWarehouseId);
        RuleFor(x => x.Lines).NotNull().NotEmpty();
        RuleForEach(x => x.Lines).ChildRules(line =>
        {
            line.RuleFor(x => x.ProductId).NotEmpty();
            line.RuleFor(x => x.Quantity).GreaterThan(0).Must(InventoryPrecision.IsQuantity);
        });
    }
}
