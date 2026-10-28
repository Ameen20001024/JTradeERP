using JTradeERP.Domain.Security;
using MediatR;
namespace JTradeERP.Application.Features.Inventory.StockBalances.GetStockBalance;
public static class GetStockBalanceEndPoint
{
    public static void Map(IEndpointRouteBuilder endpoints) => endpoints.MapGet("/stock-balances/{productId:guid}/{warehouseId:guid}",
        async (Guid productId, Guid warehouseId, ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new GetStockBalanceQuery(productId, warehouseId), ct)))
        .RequirePermission("Stock.View", Capability.View);
}
