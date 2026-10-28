using MediatR;using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.MasterData.Customers.GetCustomerById;
public static class GetCustomerByIdEndpoint{public static void Map(RouteGroupBuilder e)=>e.MapGet("/customers/{id:guid}",async(Guid id,ISender sender,CancellationToken ct)=>Results.Ok(await sender.Send(new GetCustomerByIdQuery(id),ct)));}
