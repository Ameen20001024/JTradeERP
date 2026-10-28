using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace JTradeERP.Application.Features.Security.Administration.GetAuditLogs;

public static class GetAuditLogsEndPoint
{
    public static void Map(IEndpointRouteBuilder endpoints) =>
        endpoints.MapGet("/audit-logs", ([AsParameters] GetAuditLogsQuery query, ISender sender, CancellationToken cancellationToken) =>
            sender.Send(query, cancellationToken))
            .RequirePermission("AuditLog.View", Capability.View);
}
