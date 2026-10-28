using MediatR; using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.MasterData.Products.GetProducts;
public static class GetProductsEndpoint { public static void Map(RouteGroupBuilder e)=>e.MapGet("/products",async(ISender sender,CancellationToken ct)=>Results.Ok(await sender.Send(new GetProductsQuery(),ct))); }
