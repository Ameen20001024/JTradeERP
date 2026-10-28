using MediatR;using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.MasterData.Suppliers.CreateSupplier;
public static class CreateSupplierEndpoint{public static void Map(RouteGroupBuilder e)=>e.MapPost("/suppliers",async(CreateSupplierCommand command,ISender sender,CancellationToken ct)=>{var response=await sender.Send(command,ct);return Results.Created($"/api/master-data/suppliers/{response.Id}",response);});}
