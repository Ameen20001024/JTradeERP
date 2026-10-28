using MediatR;using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.MasterData.Suppliers.GetSuppliers;
public static class GetSuppliersEndpoint{public static void Map(RouteGroupBuilder e)=>e.MapGet("/suppliers",async(ISender sender,CancellationToken ct)=>Results.Ok(await sender.Send(new GetSuppliersQuery(),ct)));}
