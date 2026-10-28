using JTradeERP.Domain.Security;using MediatR;
namespace JTradeERP.Application.Features.Inventory.Adjustments.PostStockAdjustment;
public static class PostStockAdjustmentEndPoint{public static void Map(IEndpointRouteBuilder e)=>e.MapPost("/adjustments/{id:guid}/post",async(Guid id,PostStockAdjustmentCommand c,ISender s,CancellationToken t)=>Results.Ok(await s.Send(c with{Id=id},t))).RequirePermission("Stock.Post",Capability.Post).RequirePermission("Stock.Adjust",Capability.CreateEdit);}
