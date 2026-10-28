using MediatR; using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.MasterData.TaxCodes.UpdateTaxCode;
public static class UpdateTaxCodeEndpoint { public static void Map(RouteGroupBuilder e) => e.MapPut("/tax-codes/{id:guid}", async (Guid id, UpdateTaxCodeCommand command, ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(command with { Id = id }, ct))); }
