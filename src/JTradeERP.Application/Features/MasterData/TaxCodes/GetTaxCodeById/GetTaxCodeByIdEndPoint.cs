using MediatR; using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.MasterData.TaxCodes.GetTaxCodeById;
public static class GetTaxCodeByIdEndpoint { public static void Map(RouteGroupBuilder e) => e.MapGet("/tax-codes/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new GetTaxCodeByIdQuery(id),ct))); }
