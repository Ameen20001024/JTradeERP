using MediatR; using Microsoft.AspNetCore.Builder; using Microsoft.AspNetCore.Http; using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.MasterData.Categories.CreateCategory;
public static class CreateCategoryEndpoint { public static void Map(IEndpointRouteBuilder e) => e.MapPost("/categories", async (CreateCategoryCommand command, ISender sender, CancellationToken ct) => { var response = await sender.Send(command, ct); return Results.Created($"/api/master-data/categories/{response.Id}", response); }); }
