using MediatR; using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.MasterData.TaxCodes.ActivateTaxCode;
public static class ActivateTaxCodeEndpoint { public static void Map(RouteGroupBuilder e) => e.MapPost("/tax-codes/{id:guid}/activate", async (Guid id, ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new ActivateTaxCodeCommand(id),ct))); }
