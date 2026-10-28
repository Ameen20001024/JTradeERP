using JTradeERP.Application.Common.Reports;
using MediatR;

namespace JTradeERP.Application.Features.Reports.LowStock;

public sealed class GetLowStockHandler(IReportReadService reports)
    : IRequestHandler<GetLowStockQuery, PagedReportResponse<LowStockRowResponse>>
{
    public Task<PagedReportResponse<LowStockRowResponse>> Handle(GetLowStockQuery request, CancellationToken cancellationToken) =>
        reports.GetLowStockAsync(request, cancellationToken);
}
