using MediatR;using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.MasterData.Warehouses.GetWarehouseById;
public static class GetWarehouseByIdEndpoint{public static void Map(RouteGroupBuilder e)=>e.MapGet("/warehouses/{id:guid}",async(Guid id,ISender sender,CancellationToken ct)=>Results.Ok(await sender.Send(new GetWarehouseByIdQuery(id),ct)));}
