using MediatR;using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.MasterData.Warehouses.CreateWarehouse;
public static class CreateWarehouseEndpoint{public static void Map(RouteGroupBuilder e)=>e.MapPost("/warehouses",async(CreateWarehouseCommand command,ISender sender,CancellationToken ct)=>{var response=await sender.Send(command,ct);return Results.Created($"/api/master-data/warehouses/{response.Id}",response);});}
