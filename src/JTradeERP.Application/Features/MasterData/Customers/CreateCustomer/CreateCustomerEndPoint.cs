using MediatR;using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.MasterData.Customers.CreateCustomer;
public static class CreateCustomerEndpoint{public static void Map(RouteGroupBuilder e)=>e.MapPost("/customers",async(CreateCustomerCommand command,ISender sender,CancellationToken ct)=>{var response=await sender.Send(command,ct);return Results.Created($"/api/master-data/customers/{response.Id}",response);});}
