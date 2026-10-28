using FluentValidation;

namespace JTradeERP.Application.Features.Purchasing.GoodsReceiptNotes.GetGoodsReceiptNoteById;

public sealed class GetGoodsReceiptNoteByIdValidator : AbstractValidator<GetGoodsReceiptNoteByIdQuery>
{
    public GetGoodsReceiptNoteByIdValidator() => RuleFor(x => x.Id).NotEmpty();
}
