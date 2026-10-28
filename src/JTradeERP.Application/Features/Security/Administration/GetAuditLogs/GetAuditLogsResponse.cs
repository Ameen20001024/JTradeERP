namespace JTradeERP.Application.Features.Security.Administration.GetAuditLogs;

public sealed record AuditLogItemResponse(Guid Id, Guid? ActorUserId, DateTimeOffset OccurredAt,
    string Action, string Entity, string EntityId, string? OldValue, string? NewValue, string CorrelationId);

public sealed record GetAuditLogsResponse(IReadOnlyCollection<AuditLogItemResponse> Items, int Page, int PageSize, int TotalCount);
