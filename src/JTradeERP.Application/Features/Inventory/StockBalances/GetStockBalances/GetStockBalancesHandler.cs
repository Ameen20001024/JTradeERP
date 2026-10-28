using JTradeERP.Application.Common.Inventory;
using JTradeERP.Application.Common.Persistence;
using MediatR;
namespace JTradeERP.Application.Features.Inventory.StockBalances.GetStockBalances;
public sealed class GetStockBalancesHandler(IApplicationDbContext db) : IRequestHandler<GetStockBalancesQuery, GetStockBalancesResponse>
{
    public Task<GetStockBalancesResponse> Handle(GetStockBalancesQuery request, CancellationToken cancellationToken)
    {
        var query=db.StockBalances.AsQueryable();
        if(request.ProductId.HasValue)query=query.Where(x=>x.ProductId==request.ProductId.Value);
        if(request.WarehouseId.HasValue)query=query.Where(x=>x.WarehouseId==request.WarehouseId.Value);
        var total=query.Count();
        var balances=query.OrderBy(x=>x.ProductId).ThenBy(x=>x.WarehouseId).Skip((request.Page-1)*request.PageSize).Take(request.PageSize).ToArray();
        var pids=balances.Select(x=>x.ProductId).Distinct().ToArray();var wids=balances.Select(x=>x.WarehouseId).Distinct().ToArray();
        var products=db.Products.Where(x=>pids.Contains(x.Id)).ToDictionary(x=>x.Id);var warehouses=db.Warehouses.Where(x=>wids.Contains(x.Id)).ToDictionary(x=>x.Id);
        var items=balances.Select(x=>new StockBalanceItemResponse(x.ProductId,products[x.ProductId].ProductCode,products[x.ProductId].Name,
            x.WarehouseId,warehouses[x.WarehouseId].WarehouseCode,warehouses[x.WarehouseId].Name,x.QtyOnHand,x.QtyReserved,x.AvailableQuantity,
            x.WeightedAverageCost,Convert.ToBase64String(x.RowVersion))).ToArray();
        return Task.FromResult(new GetStockBalancesResponse(items,request.Page,request.PageSize,total));
    }
}
