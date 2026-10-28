using JTradeERP.Application.Common.Persistence;
using MediatR;

namespace JTradeERP.Application.Features.Security.Administration.GetAuditLogs;

public sealed class GetAuditLogsHandler(IApplicationDbContext db)
    : IRequestHandler<GetAuditLogsQuery, GetAuditLogsResponse>
{
    public Task<GetAuditLogsResponse> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var query = db.SecurityAuditEntries.AsQueryable();
        var totalCount = query.Count();
        var items = query.OrderByDescending(x => x.OccurredAt).ThenByDescending(x => x.Id)
            .Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .Select(x => new AuditLogItemResponse(x.Id, x.ActorUserId, x.OccurredAt, x.Action,
                x.Entity, x.EntityId, x.OldValue, x.NewValue, x.CorrelationId)).ToArray();

        return Task.FromResult(new GetAuditLogsResponse(items, request.Page, request.PageSize, totalCount));
    }
}
