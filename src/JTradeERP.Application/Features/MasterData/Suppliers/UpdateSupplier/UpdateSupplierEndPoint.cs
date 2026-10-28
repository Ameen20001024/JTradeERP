using MediatR;using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.MasterData.Suppliers.UpdateSupplier;
public static class UpdateSupplierEndpoint{public static void Map(RouteGroupBuilder e)=>e.MapPut("/suppliers/{id:guid}",async(Guid id,UpdateSupplierCommand command,ISender sender,CancellationToken ct)=>Results.Ok(await sender.Send(command with{Id=id},ct)));}
