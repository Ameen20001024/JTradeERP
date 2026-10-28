using JTradeERP.Application.Common.Persistence;
using JTradeERP.Domain.Inventory;
using MediatR;
namespace JTradeERP.Application.Features.Inventory.Transfers.GetStockTransfers;
public sealed class GetStockTransfersHandler(IApplicationDbContext db)
    : IRequestHandler<GetStockTransfersQuery, GetStockTransfersResponse>
{
    public Task<GetStockTransfersResponse> Handle(GetStockTransfersQuery request, CancellationToken cancellationToken)
    {
        var query = db.StockTransfers.AsQueryable();
        if (request.SourceWarehouseId.HasValue) query = query.Where(x => x.SourceWarehouseId == request.SourceWarehouseId.Value);
        if (request.DestinationWarehouseId.HasValue) query = query.Where(x => x.DestinationWarehouseId == request.DestinationWarehouseId.Value);
        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<InventoryDocumentStatus>(request.Status, true, out var status))
            query = query.Where(x => x.Status == status);
        var total = query.Count();
        var transfers = query.OrderByDescending(x => x.Id).Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize).ToArray();
        var ids = transfers.Select(x => x.Id).ToArray();
        var linesByTransfer = db.StockTransferLines.Where(x => ids.Contains(x.StockTransferId)).ToArray()
            .GroupBy(x => x.StockTransferId).ToDictionary(x => x.Key, x => (IReadOnlyCollection<StockTransferLine>)x.ToArray());
        var items = transfers.Select(x => StockTransferResponseMapper.Map(db, x,
            linesByTransfer.GetValueOrDefault(x.Id, Array.Empty<StockTransferLine>()))).ToArray();
        return Task.FromResult(new GetStockTransfersResponse(items, request.Page, request.PageSize, total));
    }
}
