using MediatR; using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.MasterData.Products.ActivateProduct;
public static class ActivateProductEndpoint { public static void Map(RouteGroupBuilder e)=>e.MapPost("/products/{id:guid}/activate",async(Guid id,ISender sender,CancellationToken ct)=>Results.Ok(await sender.Send(new ActivateProductCommand(id),ct))); }
