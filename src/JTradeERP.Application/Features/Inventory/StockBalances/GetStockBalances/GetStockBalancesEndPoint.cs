using JTradeERP.Domain.Security;
using MediatR;
namespace JTradeERP.Application.Features.Inventory.StockBalances.GetStockBalances;
public static class GetStockBalancesEndPoint
{
    public static void Map(IEndpointRouteBuilder endpoints)=>endpoints.MapGet("/stock-balances",async(Guid? productId,Guid? warehouseId,int? page,int? pageSize,ISender sender,CancellationToken ct)=>
        Results.Ok(await sender.Send(new GetStockBalancesQuery(productId,warehouseId,page??1,pageSize??50),ct))).RequirePermission("Stock.View",Capability.View);
}
