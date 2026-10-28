using MediatR; using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.MasterData.TaxCodes.GetTaxCodes;
public static class GetTaxCodesEndpoint { public static void Map(RouteGroupBuilder e) => e.MapGet("/tax-codes", async (ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new GetTaxCodesQuery(),ct))); }
