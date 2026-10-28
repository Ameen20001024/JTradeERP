using FluentValidation;
using JTradeERP.Application.Common.Inventory;
namespace JTradeERP.Application.Features.Inventory.Transfers.UpdateStockTransfer;
public sealed class UpdateStockTransferValidator : AbstractValidator<UpdateStockTransferCommand>
{
    public UpdateStockTransferValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
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
