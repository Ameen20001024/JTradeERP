using JTradeERP.Domain.Security;using MediatR;
namespace JTradeERP.Application.Features.Inventory.Adjustments.ReverseStockAdjustment;
public static class ReverseStockAdjustmentEndPoint{public static void Map(IEndpointRouteBuilder e)=>e.MapPost("/adjustments/{id:guid}/reverse",async(Guid id,ISender s,CancellationToken t)=>Results.Ok(await s.Send(new ReverseStockAdjustmentCommand(id),t))).RequirePermission("Stock.Post",Capability.Post).RequirePermission("Stock.Adjust",Capability.CreateEdit);}
