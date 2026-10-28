namespace JTradeERP.Application.Common.Reports;

public sealed record PagedReportResponse<T>(IReadOnlyCollection<T> Items, int Page, int PageSize, long TotalCount);
