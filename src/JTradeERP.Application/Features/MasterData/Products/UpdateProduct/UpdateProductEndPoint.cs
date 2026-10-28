using MediatR; using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.MasterData.Products.UpdateProduct;
public static class UpdateProductEndpoint { public static void Map(RouteGroupBuilder e)=>e.MapPut("/products/{id:guid}",async(Guid id,UpdateProductCommand command,ISender sender,CancellationToken ct)=>Results.Ok(await sender.Send(command with { Id=id },ct))); }
