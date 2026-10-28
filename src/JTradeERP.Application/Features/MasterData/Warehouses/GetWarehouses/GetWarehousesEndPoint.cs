using MediatR;using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.MasterData.Warehouses.GetWarehouses;
public static class GetWarehousesEndpoint{public static void Map(RouteGroupBuilder e)=>e.MapGet("/warehouses",async(ISender sender,CancellationToken ct)=>Results.Ok(await sender.Send(new GetWarehousesQuery(),ct)));}
