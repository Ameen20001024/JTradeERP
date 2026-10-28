using MediatR;

namespace JTradeERP.Application.Features.Purchasing.GoodsReceiptNotes.GetGoodsReceiptNoteById;

public sealed record GetGoodsReceiptNoteByIdQuery(Guid Id) : IRequest<GetGoodsReceiptNoteByIdResponse>;
