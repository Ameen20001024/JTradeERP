using MediatR; using Microsoft.AspNetCore.Builder; using Microsoft.AspNetCore.Http; using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.Security.GetMe;
public static class GetMeEndpoint { public static void Map(IEndpointRouteBuilder e)=>e.MapGet("/me",async(ISender sender,CancellationToken ct)=>Results.Ok(await sender.Send(new GetMeQuery(),ct))).RequireAuthorization(); }
