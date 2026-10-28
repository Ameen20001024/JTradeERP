using MediatR;using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.MasterData.Warehouses.ActivateWarehouse;
public static class ActivateWarehouseEndpoint{public static void Map(RouteGroupBuilder e)=>e.MapPost("/warehouses/{id:guid}/activate",async(Guid id,ISender sender,CancellationToken ct)=>Results.Ok(await sender.Send(new ActivateWarehouseCommand(id),ct)));}
