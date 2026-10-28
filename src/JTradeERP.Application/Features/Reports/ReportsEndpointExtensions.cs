using JTradeERP.Application.Features.Reports.LowStock;
using JTradeERP.Application.Features.Reports.CustomerAging;
using JTradeERP.Application.Features.Reports.SalesSummary;
using JTradeERP.Application.Features.Reports.StockMovement;
using JTradeERP.Application.Features.Reports.StockValuation;
using Microsoft.AspNetCore.Routing;

namespace JTradeERP.Application.Features.Reports;

public static class ReportsEndpointExtensions
{
    public static IEndpointRouteBuilder MapReportEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var reports = endpoints.MapGroup("/api/reports");
        GetSalesSummaryEndPoint.Map(reports);
        GetStockValuationEndPoint.Map(reports);
        GetLowStockEndPoint.Map(reports);
        GetStockMovementEndPoint.Map(reports);
        GetCustomerAgingEndPoint.Map(reports);
        return endpoints;
    }
}
