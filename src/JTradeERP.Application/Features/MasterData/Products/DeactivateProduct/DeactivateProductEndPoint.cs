using MediatR; using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.MasterData.Products.DeactivateProduct;
public static class DeactivateProductEndpoint { public static void Map(RouteGroupBuilder e)=>e.MapPost("/products/{id:guid}/deactivate",async(Guid id,ISender sender,CancellationToken ct)=>Results.Ok(await sender.Send(new DeactivateProductCommand(id),ct))); }
