using FluentValidation;

namespace JTradeERP.Application.Features.Purchasing.GoodsReceiptNotes.PostGoodsReceiptNote;

public sealed class PostGoodsReceiptNoteValidator : AbstractValidator<PostGoodsReceiptNoteCommand>
{
    public PostGoodsReceiptNoteValidator() => RuleFor(x => x.Id).NotEmpty();
}
