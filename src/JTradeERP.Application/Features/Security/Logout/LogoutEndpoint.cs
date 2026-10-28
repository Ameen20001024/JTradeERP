using MediatR; using JTradeERP.Application.Common.Security; using Microsoft.AspNetCore.Builder; using Microsoft.AspNetCore.Http; using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.Security.Logout;
public static class LogoutEndpoint { public static void Map(IEndpointRouteBuilder e)=>e.MapPost("/logout",async(ISender sender,ICurrentUserService current,IRefreshTokenCookie cookie,CancellationToken ct)=>{await sender.Send(new LogoutCommand(current.UserId,cookie.Read()),ct);cookie.Clear();return Results.NoContent();}).RequireAuthorization(); }
