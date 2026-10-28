using MediatR;using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.MasterData.Customers.DeleteCustomer;
public static class DeleteCustomerEndpoint{public static void Map(RouteGroupBuilder e)=>e.MapDelete("/customers/{id:guid}",async(Guid id,ISender sender,CancellationToken ct)=>{await sender.Send(new DeleteCustomerCommand(id),ct);return Results.NoContent();});}
