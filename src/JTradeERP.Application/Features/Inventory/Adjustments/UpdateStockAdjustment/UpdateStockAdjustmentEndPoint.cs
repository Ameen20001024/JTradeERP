using JTradeERP.Domain.Security;using MediatR;
namespace JTradeERP.Application.Features.Inventory.Adjustments.UpdateStockAdjustment;
public static class UpdateStockAdjustmentEndPoint{public static void Map(IEndpointRouteBuilder e)=>e.MapPut("/adjustments/{id:guid}",async(Guid id,UpdateStockAdjustmentCommand c,ISender s,CancellationToken t)=>Results.Ok(await s.Send(c with{Id=id},t))).RequirePermission("Stock.Adjust",Capability.CreateEdit);}
