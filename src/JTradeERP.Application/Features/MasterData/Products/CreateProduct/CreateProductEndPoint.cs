using MediatR;
using Microsoft.AspNetCore.Routing;

namespace JTradeERP.Application.Features.MasterData.Products.CreateProduct;

public static class CreateProductEndpoint 
{
    public static void Map(RouteGroupBuilder e) => e.MapPost(
        "/products",
        async(CreateProductCommand command, ISender sender, CancellationToken ct) =>
        {
            var response = await sender.Send(command, ct);
            
            return Results.Created($"/api/master-data/products/{response.Id}",response);
        }
    );
}
