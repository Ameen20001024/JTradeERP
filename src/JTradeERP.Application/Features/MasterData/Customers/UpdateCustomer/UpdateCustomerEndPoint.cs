using MediatR;using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.MasterData.Customers.UpdateCustomer;
public static class UpdateCustomerEndpoint{public static void Map(RouteGroupBuilder e)=>e.MapPut("/customers/{id:guid}",async(Guid id,UpdateCustomerCommand command,ISender sender,CancellationToken ct)=>Results.Ok(await sender.Send(command with{Id=id},ct)));}
