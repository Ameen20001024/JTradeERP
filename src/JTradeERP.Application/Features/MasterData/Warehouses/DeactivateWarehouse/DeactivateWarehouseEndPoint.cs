using MediatR;using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.MasterData.Warehouses.DeactivateWarehouse;
public static class DeactivateWarehouseEndpoint{public static void Map(RouteGroupBuilder e)=>e.MapPost("/warehouses/{id:guid}/deactivate",async(Guid id,ISender sender,CancellationToken ct)=>Results.Ok(await sender.Send(new DeactivateWarehouseCommand(id),ct)));}
