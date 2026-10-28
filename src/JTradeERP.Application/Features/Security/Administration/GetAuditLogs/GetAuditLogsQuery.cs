using MediatR;

namespace JTradeERP.Application.Features.Security.Administration.GetAuditLogs;

public sealed record GetAuditLogsQuery(int Page = 1, int PageSize = 50) : IRequest<GetAuditLogsResponse>;
