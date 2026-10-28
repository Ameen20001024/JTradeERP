using JTradeERP.Application.Common.Reports;
using MediatR;

namespace JTradeERP.Application.Features.Reports.StockValuation;

public sealed class GetStockValuationHandler(IReportReadService reports)
    : IRequestHandler<GetStockValuationQuery, PagedReportResponse<StockValuationRowResponse>>
{
    public Task<PagedReportResponse<StockValuationRowResponse>> Handle(GetStockValuationQuery request, CancellationToken cancellationToken) =>
        reports.GetStockValuationAsync(request, cancellationToken);
}
