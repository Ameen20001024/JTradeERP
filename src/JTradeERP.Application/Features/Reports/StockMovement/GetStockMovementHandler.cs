using JTradeERP.Application.Common.Reports;
using MediatR;

namespace JTradeERP.Application.Features.Reports.StockMovement;

public sealed class GetStockMovementHandler(IReportReadService reports)
    : IRequestHandler<GetStockMovementQuery, PagedReportResponse<StockMovementRowResponse>>
{
    public Task<PagedReportResponse<StockMovementRowResponse>> Handle(GetStockMovementQuery request, CancellationToken cancellationToken) =>
        reports.GetStockMovementAsync(request, cancellationToken);
}
