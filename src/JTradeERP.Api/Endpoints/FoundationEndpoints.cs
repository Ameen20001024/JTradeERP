using MediatR;
using JTradeERP.Application.Common.Diagnostics;

namespace JTradeERP.Api.Endpoints;

public static class FoundationEndpoints
{
    public static IEndpointRouteBuilder MapFoundationEndpoints(this IEndpointRouteBuilder endpoints, bool includeDiagnostics)
    {
        var group = endpoints.MapGroup("/_foundation").AllowAnonymous();

        if (includeDiagnostics)
        {
            group.MapGet("/throw", (HttpContext _) => throw new InvalidOperationException("Foundation test exception."));
            group.MapGet("/validation", async (string? value, ISender sender, CancellationToken cancellationToken) =>
                Results.Ok(await sender.Send(new FoundationValidationRequest(value ?? string.Empty), cancellationToken)));
        }

        return endpoints;
    }
}
