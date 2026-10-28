using MediatR; using JTradeERP.Application.Common.Security; using Microsoft.AspNetCore.Builder; using Microsoft.AspNetCore.Http; using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.Security.RefreshAccessToken;
public static class RefreshAccessTokenEndpoint { public static void Map(IEndpointRouteBuilder e)=>e.MapPost("/refresh",async(ISender sender,IRefreshTokenCookie cookie,CancellationToken ct)=>Results.Ok(await sender.Send(new RefreshAccessTokenCommand(cookie.Read()),ct))).AllowAnonymous(); }
