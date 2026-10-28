using MediatR; using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.MasterData.TaxCodes.DeactivateTaxCode;
public static class DeactivateTaxCodeEndpoint { public static void Map(RouteGroupBuilder e) => e.MapPost("/tax-codes/{id:guid}/deactivate", async (Guid id, ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new DeactivateTaxCodeCommand(id),ct))); }
