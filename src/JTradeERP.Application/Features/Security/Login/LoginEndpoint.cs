using MediatR; using Microsoft.AspNetCore.Builder; using Microsoft.AspNetCore.Http; using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.Security.Login;
public static class LoginEndpoint { public static void Map(IEndpointRouteBuilder e)=>e.MapPost("/login",async(LoginCommand command,ISender sender,CancellationToken ct)=>Results.Ok(await sender.Send(command,ct))).AllowAnonymous(); }
