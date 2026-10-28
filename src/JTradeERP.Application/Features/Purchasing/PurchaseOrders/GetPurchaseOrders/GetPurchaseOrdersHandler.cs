using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Features.Purchasing.Common;
using MediatR;

namespace JTradeERP.Application.Features.Purchasing.PurchaseOrders.GetPurchaseOrders;

public sealed class GetPurchaseOrdersHandler(IApplicationDbContext db)
    : IRequestHandler<GetPurchaseOrdersQuery, GetPurchaseOrdersResponse>
{
    public Task<GetPurchaseOrdersResponse> Handle(GetPurchaseOrdersQuery request, CancellationToken cancellationToken)
    {
        var query = db.PurchaseOrders.AsQueryable();
        if (request.SupplierId.HasValue) query = query.Where(x => x.SupplierId == request.SupplierId.Value);
        if (request.WarehouseId.HasValue) query = query.Where(x => x.WarehouseId == request.WarehouseId.Value);
        if (request.Status.HasValue) query = query.Where(x => x.Status == request.Status.Value);
        var total = query.Count();
        var items = query.OrderByDescending(x => x.OrderDate).ThenBy(x => x.Id)
            .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
            .Select(x => new PurchaseOrderSummaryResponse(x.Id, x.SupplierId, x.WarehouseId,
                x.OrderDate, x.ExpectedDate, x.Status.ToString(), x.GrandTotal)).ToArray();
        return Task.FromResult(new GetPurchaseOrdersResponse(items, request.PageNumber, request.PageSize, total));
    }
}
