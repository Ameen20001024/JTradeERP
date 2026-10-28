using JTradeERP.Application.Common.Reports;
using MediatR;

namespace JTradeERP.Application.Features.Reports.LowStock;

public sealed record GetLowStockQuery(Guid? WarehouseId = null, Guid? ProductId = null,
    Guid? CategoryId = null, int Page = 1, int PageSize = 50, string SortBy = "available", string SortDirection = "asc")
    : IRequest<PagedReportResponse<LowStockRowResponse>>;
