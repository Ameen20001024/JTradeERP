using MediatR;using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.MasterData.Warehouses.UpdateWarehouse;
public static class UpdateWarehouseEndpoint{public static void Map(RouteGroupBuilder e)=>e.MapPut("/warehouses/{id:guid}",async(Guid id,UpdateWarehouseCommand command,ISender sender,CancellationToken ct)=>Results.Ok(await sender.Send(command with{Id=id},ct)));}
