using FluentValidation;

namespace JTradeERP.Application.Features.Purchasing.GoodsReceiptNotes.GetGoodsReceiptNotes;

public sealed class GetGoodsReceiptNotesValidator : AbstractValidator<GetGoodsReceiptNotesQuery>
{
    public GetGoodsReceiptNotesValidator()
    {
        RuleFor(x => x.PurchaseOrderId).Must(x => !x.HasValue || x.Value != Guid.Empty);
        RuleFor(x => x.SupplierId).Must(x => !x.HasValue || x.Value != Guid.Empty);
        RuleFor(x => x.WarehouseId).Must(x => !x.HasValue || x.Value != Guid.Empty);
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
    }
}
