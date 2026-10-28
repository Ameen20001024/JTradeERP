using JTradeERP.Application.Features.Reports.LowStock;
using JTradeERP.Application.Features.Reports.SalesSummary;
using JTradeERP.Application.Features.Reports.StockMovement;
using JTradeERP.Application.Features.Reports.StockValuation;
using JTradeERP.Application.Features.Reports.CustomerAging;

namespace JTradeERP.Application.Common.Reports;

public interface IReportReadService
{
    Task<PagedReportResponse<SalesSummaryRowResponse>> GetSalesSummaryAsync(GetSalesSummaryQuery query, CancellationToken cancellationToken);
    Task<PagedReportResponse<StockValuationRowResponse>> GetStockValuationAsync(GetStockValuationQuery query, CancellationToken cancellationToken);
    Task<PagedReportResponse<LowStockRowResponse>> GetLowStockAsync(GetLowStockQuery query, CancellationToken cancellationToken);
    Task<PagedReportResponse<StockMovementRowResponse>> GetStockMovementAsync(GetStockMovementQuery query, CancellationToken cancellationToken);
    Task<PagedReportResponse<CustomerAgingRowResponse>> GetCustomerAgingAsync(GetCustomerAgingQuery query, CancellationToken cancellationToken);
}
