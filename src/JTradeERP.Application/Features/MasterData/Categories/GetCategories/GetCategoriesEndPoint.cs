using MediatR; using Microsoft.AspNetCore.Http; using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.MasterData.Categories.GetCategories;
public static class GetCategoriesEndpoint { public static void Map(IEndpointRouteBuilder e) => e.MapGet("/categories", async (ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new GetCategoriesQuery(), ct))); }
