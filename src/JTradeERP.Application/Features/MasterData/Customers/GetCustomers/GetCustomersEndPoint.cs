using MediatR;using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.MasterData.Customers.GetCustomers;
public static class GetCustomersEndpoint{public static void Map(RouteGroupBuilder e)=>e.MapGet("/customers",async(ISender sender,CancellationToken ct)=>Results.Ok(await sender.Send(new GetCustomersQuery(),ct)));}
