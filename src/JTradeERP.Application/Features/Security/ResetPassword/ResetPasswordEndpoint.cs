using MediatR; using Microsoft.AspNetCore.Builder; using Microsoft.AspNetCore.Http; using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.Security.ResetPassword;
public static class ResetPasswordEndpoint { public static void Map(IEndpointRouteBuilder e)=>e.MapPost("/reset-password",async(ResetPasswordCommand command,ISender sender,CancellationToken ct)=>Results.Ok(await sender.Send(command,ct))).AllowAnonymous(); }
