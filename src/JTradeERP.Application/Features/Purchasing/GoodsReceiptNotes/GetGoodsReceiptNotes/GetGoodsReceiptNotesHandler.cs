using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Features.Purchasing.Common;
using MediatR;

namespace JTradeERP.Application.Features.Purchasing.GoodsReceiptNotes.GetGoodsReceiptNotes;

public sealed class GetGoodsReceiptNotesHandler(IApplicationDbContext db)
    : IRequestHandler<GetGoodsReceiptNotesQuery, GetGoodsReceiptNotesResponse>
{
    public Task<GetGoodsReceiptNotesResponse> Handle(GetGoodsReceiptNotesQuery request, CancellationToken cancellationToken)
    {
        var query = db.GoodsReceiptNotes.AsQueryable();
        if (request.PurchaseOrderId.HasValue) query = query.Where(x => x.PurchaseOrderId == request.PurchaseOrderId.Value);
        if (request.SupplierId.HasValue) query = query.Where(x => x.SupplierId == request.SupplierId.Value);
        if (request.WarehouseId.HasValue) query = query.Where(x => x.WarehouseId == request.WarehouseId.Value);
        if (request.Status.HasValue) query = query.Where(x => x.Status == request.Status.Value);
        var total = query.Count();
        var items = query.OrderByDescending(x => x.ReceiptDate).ThenBy(x => x.Id)
            .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
            .Select(x => new GoodsReceiptNoteSummaryResponse(x.Id, x.PurchaseOrderId, x.SupplierId,
                x.WarehouseId, x.ReceiptDate, x.Status.ToString(), x.Lines.Count)).ToArray();
        return Task.FromResult(new GetGoodsReceiptNotesResponse(items, request.PageNumber, request.PageSize, total));
    }
}
