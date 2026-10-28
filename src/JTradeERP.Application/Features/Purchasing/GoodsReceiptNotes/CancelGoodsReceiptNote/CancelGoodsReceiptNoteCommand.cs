using MediatR;

namespace JTradeERP.Application.Features.Purchasing.GoodsReceiptNotes.CancelGoodsReceiptNote;

public sealed record CancelGoodsReceiptNoteCommand(Guid Id) : IRequest<CancelGoodsReceiptNoteResponse>;
