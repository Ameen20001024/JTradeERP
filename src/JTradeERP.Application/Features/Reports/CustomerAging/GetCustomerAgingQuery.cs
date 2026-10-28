using JTradeERP.Application.Common.Reports;
using MediatR;

namespace JTradeERP.Application.Features.Reports.CustomerAging;

public sealed record GetCustomerAgingQuery(
    DateOnly AsOfDate,
    Guid? CustomerId = null,
    int Page = 1,
    int PageSize = 50,
    string SortBy = "customer",
    string SortDirection = "asc") : IRequest<PagedReportResponse<CustomerAgingRowResponse>>;
