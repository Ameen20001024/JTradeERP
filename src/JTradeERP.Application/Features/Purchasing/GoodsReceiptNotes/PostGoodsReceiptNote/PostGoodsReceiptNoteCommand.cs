using MediatR;

namespace JTradeERP.Application.Features.Purchasing.GoodsReceiptNotes.PostGoodsReceiptNote;

public sealed record PostGoodsReceiptNoteCommand(Guid Id) : IRequest<PostGoodsReceiptNoteResponse>;
