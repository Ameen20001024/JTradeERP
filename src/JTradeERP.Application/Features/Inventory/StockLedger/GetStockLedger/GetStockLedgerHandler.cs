using JTradeERP.Application.Common.Inventory;
using JTradeERP.Application.Common.Persistence;
using MediatR;
namespace JTradeERP.Application.Features.Inventory.StockLedger.GetStockLedger;
public sealed class GetStockLedgerHandler(IApplicationDbContext db):IRequestHandler<GetStockLedgerQuery,GetStockLedgerResponse>
{
    public Task<GetStockLedgerResponse> Handle(GetStockLedgerQuery r,CancellationToken ct)
    {
        var q = db.StockLedgerEntries.AsQueryable();

        if(r.ProductId.HasValue)
            q = q.Where(x => x.ProductId == r.ProductId.Value);
        
        if(r.WarehouseId.HasValue)
            q = q.Where(x => x.WarehouseId == r.WarehouseId.Value);
        
        if(r.From.HasValue)
            q = q.Where(x => x.PostedAt >= r.From.Value);
        
        if(r.To.HasValue)
            q = q.Where(x => x.PostedAt <= r.To.Value);

        if(!string.IsNullOrWhiteSpace(r.SourceDocumentType))
            q = q.Where(x => x.SourceDocumentType == r.SourceDocumentType);
        
        var total = q.Count();
        
        var items = q.OrderByDescending(x=>x.PostedAt)
            .ThenBy(x=>x.Id)
            .Skip((r.Page-1)*r.PageSize)
            .Take(r.PageSize)
            .Select(x=>new StockLedgerItemResponse(
                x.Id,
                x.ProductId,
                x.WarehouseId,
                x.MovementType.ToString(),
                x.Direction.ToString(),
                x.Quantity,
                x.UnitCost,
                x.SourceDocumentType,
                x.SourceDocumentId,
                x.SourceLineId,
                x.ReversalOfLedgerEntryId,
                x.PostedAt,
                x.PostedByUserId,
                x.CorrelationId)).ToArray();

        return Task.FromResult(new GetStockLedgerResponse(items, r.Page, r.PageSize, total));
    }
}
