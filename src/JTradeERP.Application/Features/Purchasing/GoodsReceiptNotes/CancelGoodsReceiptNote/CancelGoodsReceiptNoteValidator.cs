using FluentValidation;

namespace JTradeERP.Application.Features.Purchasing.GoodsReceiptNotes.CancelGoodsReceiptNote;

public sealed class CancelGoodsReceiptNoteValidator : AbstractValidator<CancelGoodsReceiptNoteCommand>
{
    public CancelGoodsReceiptNoteValidator() => RuleFor(x => x.Id).NotEmpty();
}
