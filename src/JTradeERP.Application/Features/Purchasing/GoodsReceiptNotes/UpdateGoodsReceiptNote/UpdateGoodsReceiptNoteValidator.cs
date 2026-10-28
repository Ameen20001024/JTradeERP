using FluentValidation;
using JTradeERP.Application.Features.Purchasing.Common;

namespace JTradeERP.Application.Features.Purchasing.GoodsReceiptNotes.UpdateGoodsReceiptNote;

public sealed class UpdateGoodsReceiptNoteValidator : AbstractValidator<UpdateGoodsReceiptNoteCommand>
{
    public UpdateGoodsReceiptNoteValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.PurchaseOrderId).Must(x => !x.HasValue || x.Value != Guid.Empty);
        RuleFor(x => x.SupplierId).NotEmpty();
        RuleFor(x => x.WarehouseId).NotEmpty();
        RuleFor(x => x.ReceiptDate).NotEmpty();
        RuleFor(x => x.Lines).NotNull().NotEmpty();
        RuleForEach(x => x.Lines).ChildRules(line =>
        {
            line.RuleFor(x => x.ProductId).NotEmpty();
            line.RuleFor(x => x.PurchaseOrderLineId).Must(x => !x.HasValue || x.Value != Guid.Empty);
            line.RuleFor(x => x.ReceivedQuantity).GreaterThan(0).Must(PurchasingPrecision.Quantity);
            line.RuleFor(x => x.UnitCost).Must(x => !x.HasValue || (x.Value >= 0 && PurchasingPrecision.UnitCost(x.Value)));
            line.RuleFor(x => x.TaxCodeId).Must(x => !x.HasValue || x.Value != Guid.Empty);
        });
    }
}
