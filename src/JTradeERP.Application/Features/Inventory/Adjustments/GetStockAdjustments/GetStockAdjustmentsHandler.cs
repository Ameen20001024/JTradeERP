using JTradeERP.Application.Common.Inventory;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Domain.Inventory;
using MediatR;

namespace JTradeERP.Application.Features.Inventory.Adjustments.GetStockAdjustments;

public sealed class GetStockAdjustmentsHandler(IApplicationDbContext db)
    : IRequestHandler<GetStockAdjustmentsQuery, GetStockAdjustmentsResponse>
{
    public Task<GetStockAdjustmentsResponse> Handle(GetStockAdjustmentsQuery request, CancellationToken cancellationToken)
    {
        var query = db.StockAdjustments.AsQueryable();
        if (request.ProductId.HasValue) query = query.Where(x => x.ProductId == request.ProductId.Value);
        if (request.WarehouseId.HasValue) query = query.Where(x => x.WarehouseId == request.WarehouseId.Value);
        if (!string.IsNullOrWhiteSpace(request.Status)
            && Enum.TryParse<InventoryDocumentStatus>(request.Status, true, out var status))
            query = query.Where(x => x.Status == status);

        var total = query.Count();
        var items = query.OrderByDescending(x => x.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToArray()
            .Select(StockAdjustmentMapper.ToResponse)
            .ToArray();
        return Task.FromResult(new GetStockAdjustmentsResponse(items, request.Page, request.PageSize, total));
    }
}
