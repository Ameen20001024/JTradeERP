using JTradeERP.Application.Common.Reports;
using MediatR;

namespace JTradeERP.Application.Features.Reports.SalesSummary;

public sealed record GetSalesSummaryQuery(DateOnly? FromDate = null, DateOnly? ToDate = null,
    Guid? CustomerId = null, Guid? ProductId = null, int Page = 1, int PageSize = 50,
    string SortBy = "date", string SortDirection = "asc") : IRequest<PagedReportResponse<SalesSummaryRowResponse>>;
