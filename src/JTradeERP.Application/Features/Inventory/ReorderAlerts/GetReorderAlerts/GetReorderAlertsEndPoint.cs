using JTradeERP.Domain.Security;
using MediatR;
namespace JTradeERP.Application.Features.Inventory.ReorderAlerts.GetReorderAlerts;
public static class GetReorderAlertsEndPoint
{
    public static void Map(IEndpointRouteBuilder e)=>e.MapGet("/reorder-alerts",async(Guid? warehouseId,ISender s,CancellationToken ct)=>Results.Ok(await s.Send(new GetReorderAlertsQuery(warehouseId),ct))).RequirePermission("Stock.View",Capability.View);
}
