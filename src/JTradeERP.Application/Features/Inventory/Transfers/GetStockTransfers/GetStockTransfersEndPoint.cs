using JTradeERP.Domain.Security;
using MediatR;
namespace JTradeERP.Application.Features.Inventory.Transfers.GetStockTransfers;
public static class GetStockTransfersEndPoint
{
    public static void Map(IEndpointRouteBuilder endpoints) => endpoints.MapGet("/transfers",
        async (Guid? sourceWarehouseId, Guid? destinationWarehouseId, string? status, int? page, int? pageSize,
            ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new GetStockTransfersQuery(
                sourceWarehouseId, destinationWarehouseId, status, page ?? 1, pageSize ?? 50), ct)))
        .RequirePermission("Stock.View", Capability.View);
}
