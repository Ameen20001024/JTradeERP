using JTradeERP.Application.Common.Reports;
using MediatR;

namespace JTradeERP.Application.Features.Reports.StockValuation;

public sealed record GetStockValuationQuery(Guid? WarehouseId = null, Guid? ProductId = null,
    Guid? CategoryId = null, int Page = 1, int PageSize = 50, string SortBy = "warehouse", string SortDirection = "asc")
    : IRequest<PagedReportResponse<StockValuationRowResponse>>;
