using MediatR; using Microsoft.AspNetCore.Http; using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.MasterData.Categories.UpdateCategory;
public static class UpdateCategoryEndpoint { public static void Map(IEndpointRouteBuilder e) => e.MapPut("/categories/{id:guid}", async (Guid id, UpdateCategoryCommand command, ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(command with { Id = id }, ct))); }
