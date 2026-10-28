using MediatR; using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.MasterData.TaxCodes.CreateTaxCode;
public static class CreateTaxCodeEndpoint { public static void Map(RouteGroupBuilder e) => e.MapPost("/tax-codes", async (CreateTaxCodeCommand command, ISender sender, CancellationToken ct) => { var response = await sender.Send(command, ct); return Results.Created($"/api/master-data/tax-codes/{response.Id}", response); }); }
