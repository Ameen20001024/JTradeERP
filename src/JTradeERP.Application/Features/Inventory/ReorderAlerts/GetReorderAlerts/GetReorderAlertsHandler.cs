using JTradeERP.Application.Common.Inventory;
using JTradeERP.Application.Common.Persistence;
using MediatR;
namespace JTradeERP.Application.Features.Inventory.ReorderAlerts.GetReorderAlerts;
public sealed class GetReorderAlertsHandler(IApplicationDbContext db):IRequestHandler<GetReorderAlertsQuery,GetReorderAlertsResponse>
{
    public Task<GetReorderAlertsResponse> Handle(GetReorderAlertsQuery r,CancellationToken ct)
    {
        var products=db.Products.ToArray();var warehouses=db.Warehouses.Where(x=>!r.WarehouseId.HasValue||x.Id==r.WarehouseId.Value).ToArray();
        var balances=db.StockBalances.Where(x=>!r.WarehouseId.HasValue||x.WarehouseId==r.WarehouseId.Value).ToDictionary(x=>(x.ProductId,x.WarehouseId));
        var alerts=new List<ReorderAlertItemResponse>();
        foreach(var product in products)foreach(var warehouse in warehouses){balances.TryGetValue((product.Id,warehouse.Id),out var balance);var onHand=balance?.QtyOnHand??0;var reserved=balance?.QtyReserved??0;var available=onHand-reserved;
            if(available<product.MinimumStockLevel)alerts.Add(new(product.Id,product.ProductCode,product.Name,warehouse.Id,warehouse.WarehouseCode,warehouse.Name,onHand,reserved,available,product.MinimumStockLevel,product.ReorderQuantity));}
        return Task.FromResult(new GetReorderAlertsResponse(alerts.OrderBy(x=>x.ProductCode).ThenBy(x=>x.WarehouseCode).ToArray(),alerts.Count));
    }
}
