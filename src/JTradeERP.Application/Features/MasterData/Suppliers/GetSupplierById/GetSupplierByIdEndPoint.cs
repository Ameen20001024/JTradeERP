using MediatR;using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.MasterData.Suppliers.GetSupplierById;
public static class GetSupplierByIdEndpoint{public static void Map(RouteGroupBuilder e)=>e.MapGet("/suppliers/{id:guid}",async(Guid id,ISender sender,CancellationToken ct)=>Results.Ok(await sender.Send(new GetSupplierByIdQuery(id),ct)));}
