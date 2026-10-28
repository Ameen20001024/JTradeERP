using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Features.Purchasing.Common;
using MediatR;

namespace JTradeERP.Application.Features.Purchasing.GoodsReceiptNotes.GetGoodsReceiptNoteById;

public sealed class GetGoodsReceiptNoteByIdHandler(IApplicationDbContext db)
    : IRequestHandler<GetGoodsReceiptNoteByIdQuery, GetGoodsReceiptNoteByIdResponse>
{
    public Task<GetGoodsReceiptNoteByIdResponse> Handle(GetGoodsReceiptNoteByIdQuery request, CancellationToken cancellationToken)
    {
        var note = db.GoodsReceiptNotesWithLines.SingleOrDefault(x => x.Id == request.Id)
            ?? throw new NotFoundException("Goods receipt note was not found.");
        return Task.FromResult(new GetGoodsReceiptNoteByIdResponse(PurchasingResponseMapper.Map(note)));
    }
}
