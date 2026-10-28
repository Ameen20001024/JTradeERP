using MediatR;using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.MasterData.Suppliers.DeleteSupplier;
public static class DeleteSupplierEndpoint{public static void Map(RouteGroupBuilder e)=>e.MapDelete("/suppliers/{id:guid}",async(Guid id,ISender sender,CancellationToken ct)=>{await sender.Send(new DeleteSupplierCommand(id),ct);return Results.NoContent();});}
