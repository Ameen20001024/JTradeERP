using JTradeERP.Application.Common.Reports;
using MediatR;

namespace JTradeERP.Application.Features.Reports.SalesSummary;

public sealed class GetSalesSummaryHandler(IReportReadService reports)
    : IRequestHandler<GetSalesSummaryQuery, PagedReportResponse<SalesSummaryRowResponse>>
{
    public Task<PagedReportResponse<SalesSummaryRowResponse>> Handle(GetSalesSummaryQuery request, CancellationToken cancellationToken) =>
        reports.GetSalesSummaryAsync(request, cancellationToken);
}
