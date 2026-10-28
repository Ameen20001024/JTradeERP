using JTradeERP.Domain.Security;using MediatR;
namespace JTradeERP.Application.Features.Inventory.Adjustments.GetStockAdjustment;
public static class GetStockAdjustmentEndPoint{public static void Map(IEndpointRouteBuilder e)=>e.MapGet("/adjustments/{id:guid}",async(Guid id,ISender s,CancellationToken t)=>Results.Ok(await s.Send(new GetStockAdjustmentQuery(id),t))).RequirePermission("Stock.View",Capability.View);}
