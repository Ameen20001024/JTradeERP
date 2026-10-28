using JTradeERP.Application.Common.Reports;
using MediatR;

namespace JTradeERP.Application.Features.Reports.CustomerAging;

public sealed class GetCustomerAgingHandler(IReportReadService reports)
    : IRequestHandler<GetCustomerAgingQuery, PagedReportResponse<CustomerAgingRowResponse>>
{
    public Task<PagedReportResponse<CustomerAgingRowResponse>> Handle(GetCustomerAgingQuery request, CancellationToken cancellationToken) =>
        reports.GetCustomerAgingAsync(request, cancellationToken);
}
