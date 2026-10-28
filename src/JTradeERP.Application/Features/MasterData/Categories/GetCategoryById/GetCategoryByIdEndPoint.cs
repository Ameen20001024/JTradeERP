using MediatR; using Microsoft.AspNetCore.Http; using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.MasterData.Categories.GetCategoryById;
public static class GetCategoryByIdEndpoint { public static void Map(IEndpointRouteBuilder e) => e.MapGet("/categories/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new GetCategoryByIdQuery(id), ct))); }
