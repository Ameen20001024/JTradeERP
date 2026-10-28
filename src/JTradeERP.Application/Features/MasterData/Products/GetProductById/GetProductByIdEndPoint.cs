using MediatR; using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.MasterData.Products.GetProductById;
public static class GetProductByIdEndpoint { public static void Map(RouteGroupBuilder e)=>e.MapGet("/products/{id:guid}",async(Guid id,ISender sender,CancellationToken ct)=>Results.Ok(await sender.Send(new GetProductByIdQuery(id),ct))); }
