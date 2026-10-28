using Dapper;
using System.Data;
using JTradeERP.Application.Common.Reports;
using JTradeERP.Application.Features.Reports.LowStock;
using JTradeERP.Application.Features.Reports.SalesSummary;
using JTradeERP.Application.Features.Reports.StockMovement;
using JTradeERP.Application.Features.Reports.StockValuation;
using JTradeERP.Application.Features.Reports.CustomerAging;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace JTradeERP.Infrastructure.Reports;

public sealed class DapperReportReadService(IConfiguration configuration) : IReportReadService
{
    static DapperReportReadService() => SqlMapper.AddTypeHandler(new SqlDateOnlyTypeHandler());
    private const string SalesSql = """
        WITH Filtered AS (
            SELECT i.InvoiceDate AS [Date], c.Id AS CustomerId, c.Name AS CustomerName,
                   p.Id AS ProductId, p.ProductCode, p.Name AS ProductName,
                   SUM(l.LineSubTotal) AS SubTotal, SUM(l.DiscountAmount) AS DiscountAmount,
                   SUM(l.LineSubTotal - l.DiscountAmount) AS NetAmount,
                   SUM(l.TaxAmountSnapshot) AS TaxAmount, SUM(l.LineTotal) AS GrandTotal
            FROM Invoices AS i
            INNER JOIN InvoiceLines AS l ON l.InvoiceId = i.Id
            INNER JOIN Customers AS c ON c.Id = i.CustomerId
            INNER JOIN Products AS p ON p.Id = l.ProductId
            WHERE i.Status IN (2, 3, 4)
              AND (@FromDate IS NULL OR i.InvoiceDate >= @FromDate)
              AND (@ToDate IS NULL OR i.InvoiceDate <= @ToDate)
              AND (@CustomerId IS NULL OR i.CustomerId = @CustomerId)
              AND (@ProductId IS NULL OR l.ProductId = @ProductId)
            GROUP BY i.InvoiceDate, c.Id, c.Name, p.Id, p.ProductCode, p.Name
        )
        """;

    private const string ValuationSql = """
        WITH Filtered AS (
            SELECT w.Id AS WarehouseId, w.WarehouseCode, w.Name AS WarehouseName,
                   p.Id AS ProductId, p.ProductCode, p.Name AS ProductName,
                   c.Id AS CategoryId, c.Name AS CategoryName, b.QtyOnHand,
                   b.WeightedAverageCost,
                   CAST(b.QtyOnHand * b.WeightedAverageCost AS decimal(28, 4)) AS InventoryValue
            FROM StockBalances AS b
            INNER JOIN Products AS p ON p.Id = b.ProductId
            INNER JOIN Categories AS c ON c.Id = p.CategoryId
            INNER JOIN Warehouses AS w ON w.Id = b.WarehouseId
            WHERE (@WarehouseId IS NULL OR b.WarehouseId = @WarehouseId)
              AND (@ProductId IS NULL OR b.ProductId = @ProductId)
              AND (@CategoryId IS NULL OR p.CategoryId = @CategoryId)
        )
        """;

    private const string LowStockSql = """
        WITH Filtered AS (
            SELECT w.Id AS WarehouseId, w.WarehouseCode, w.Name AS WarehouseName,
                   p.Id AS ProductId, p.ProductCode, p.Name AS ProductName,
                   c.Id AS CategoryId, c.Name AS CategoryName, b.QtyOnHand, b.QtyReserved,
                   b.QtyOnHand - b.QtyReserved AS AvailableQuantity,
                   p.MinimumStockLevel, p.ReorderQuantity
            FROM StockBalances AS b
            INNER JOIN Products AS p ON p.Id = b.ProductId
            INNER JOIN Categories AS c ON c.Id = p.CategoryId
            INNER JOIN Warehouses AS w ON w.Id = b.WarehouseId
            WHERE p.MinimumStockLevel > 0
              AND b.QtyOnHand - b.QtyReserved < p.MinimumStockLevel
              AND (@WarehouseId IS NULL OR b.WarehouseId = @WarehouseId)
              AND (@ProductId IS NULL OR b.ProductId = @ProductId)
              AND (@CategoryId IS NULL OR p.CategoryId = @CategoryId)
        )
        """;

    private const string MovementSql = """
        WITH Filtered AS (
            SELECT l.Id AS LedgerEntryId, l.PostedAt, w.Id AS WarehouseId, w.WarehouseCode,
                   w.Name AS WarehouseName, p.Id AS ProductId, p.ProductCode, p.Name AS ProductName,
                   CASE l.MovementType WHEN 1 THEN 'OpeningBalance' WHEN 2 THEN 'Adjustment' WHEN 3 THEN 'Transfer' WHEN 4 THEN 'Receipt' WHEN 5 THEN 'Issue' WHEN 6 THEN 'Reversal' END AS MovementType,
                   CASE l.Direction WHEN 1 THEN 'In' WHEN 2 THEN 'Out' END AS Direction,
                   l.Quantity, l.UnitCost, l.SourceDocumentType,
                   l.SourceDocumentId, l.SourceLineId, l.ReversalOfLedgerEntryId, l.CorrelationId
            FROM StockLedgerEntries AS l
            INNER JOIN Products AS p ON p.Id = l.ProductId
            INNER JOIN Warehouses AS w ON w.Id = l.WarehouseId
            WHERE (@FromUtc IS NULL OR l.PostedAt >= @FromUtc)
              AND (@ToUtc IS NULL OR l.PostedAt <= @ToUtc)
              AND (@WarehouseId IS NULL OR l.WarehouseId = @WarehouseId)
              AND (@ProductId IS NULL OR l.ProductId = @ProductId)
              AND (@MovementType IS NULL OR l.MovementType = @MovementType)
              AND (@SourceDocumentType IS NULL OR l.SourceDocumentType = @SourceDocumentType)
        )
        """;

    // Reconstructs the outstanding balance as it stood at the end of AsOfDate. Current
    // invoice/payment status alone is insufficient because later posting, cancellation,
    // payment, or reversal activity must not rewrite a historical report.
    private const string CustomerAgingSql = """
        WITH InvoiceOutstanding AS (
            SELECT i.CustomerId, c.CustomerCode, c.Name AS CustomerName, i.InvoiceDate,
                   CASE WHEN DATEDIFF(day, i.InvoiceDate, @AsOfDate) <= 0 THEN 1
                        WHEN DATEDIFF(day, i.InvoiceDate, @AsOfDate) <= 30 THEN 2
                        WHEN DATEDIFF(day, i.InvoiceDate, @AsOfDate) <= 60 THEN 3
                        WHEN DATEDIFF(day, i.InvoiceDate, @AsOfDate) <= 90 THEN 4 ELSE 5 END AS AgingBucket,
                   CAST(i.GrandTotal - COALESCE(a.AllocatedAmount, 0) AS decimal(19, 4)) AS Outstanding
            FROM Invoices AS i
            INNER JOIN Customers AS c ON c.Id = i.CustomerId
            OUTER APPLY (
                SELECT SUM(pa.Amount) AS AllocatedAmount
                FROM PaymentAllocations AS pa
                INNER JOIN Payments AS p ON p.Id = pa.PaymentId
                WHERE pa.InvoiceId = i.Id
                  AND p.CreatedAt < @AsOfEndExclusive
                  AND p.PaymentDate <= @AsOfDate
                  AND (p.Status = 1 OR (p.Status = 2 AND p.ReversedAt >= @AsOfEndExclusive))
            ) AS a
            WHERE (@CustomerId IS NULL OR i.CustomerId = @CustomerId)
              AND i.PostedAt < @AsOfEndExclusive
              AND (i.Status IN (2, 3, 4) OR (i.Status = 5 AND i.CancelledAt >= @AsOfEndExclusive))
        ),
        Filtered AS (
            SELECT CustomerId, CustomerCode, CustomerName, @AsOfDate AS AsOfDate,
                   CAST(SUM(CASE WHEN AgingBucket = 1 THEN Outstanding ELSE 0 END) AS decimal(19, 4)) AS [Current],
                   CAST(SUM(CASE WHEN AgingBucket = 2 THEN Outstanding ELSE 0 END) AS decimal(19, 4)) AS Days1To30,
                   CAST(SUM(CASE WHEN AgingBucket = 3 THEN Outstanding ELSE 0 END) AS decimal(19, 4)) AS Days31To60,
                   CAST(SUM(CASE WHEN AgingBucket = 4 THEN Outstanding ELSE 0 END) AS decimal(19, 4)) AS Days61To90,
                   CAST(SUM(CASE WHEN AgingBucket = 5 THEN Outstanding ELSE 0 END) AS decimal(19, 4)) AS Days91Plus,
                   CAST(SUM(Outstanding) AS decimal(19, 4)) AS TotalOutstanding
            FROM InvoiceOutstanding
            WHERE Outstanding > 0
            GROUP BY CustomerId, CustomerCode, CustomerName
        )
        """;

    private readonly string connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection must be configured.");

    public Task<PagedReportResponse<SalesSummaryRowResponse>> GetSalesSummaryAsync(GetSalesSummaryQuery query, CancellationToken cancellationToken)
    {
        var parameters = new
        {
            FromDate = query.FromDate?.ToDateTime(TimeOnly.MinValue),
            ToDate = query.ToDate?.ToDateTime(TimeOnly.MinValue),
            query.CustomerId,
            query.ProductId
        };
        return ReadPageAsync<SalesSummaryRowResponse>(SalesSql, parameters, SalesOrder(query.SortBy), query.SortDirection,
            SalesStableOrder(query.SortBy), query.Page, query.PageSize, cancellationToken);
    }

    public Task<PagedReportResponse<StockValuationRowResponse>> GetStockValuationAsync(GetStockValuationQuery query, CancellationToken cancellationToken)
    {
        var parameters = new { query.WarehouseId, query.ProductId, query.CategoryId };
        return ReadPageAsync<StockValuationRowResponse>(ValuationSql, parameters, ValuationOrder(query.SortBy), query.SortDirection,
            "WarehouseId, ProductId", query.Page, query.PageSize, cancellationToken);
    }

    public Task<PagedReportResponse<LowStockRowResponse>> GetLowStockAsync(GetLowStockQuery query, CancellationToken cancellationToken)
    {
        var parameters = new { query.WarehouseId, query.ProductId, query.CategoryId };
        return ReadPageAsync<LowStockRowResponse>(LowStockSql, parameters, LowStockOrder(query.SortBy), query.SortDirection,
            "WarehouseId, ProductId", query.Page, query.PageSize, cancellationToken);
    }

    public Task<PagedReportResponse<StockMovementRowResponse>> GetStockMovementAsync(GetStockMovementQuery query, CancellationToken cancellationToken)
    {
        var parameters = new
        {
            query.FromUtc,
            query.ToUtc,
            query.WarehouseId,
            query.ProductId,
            MovementType = query.MovementType.HasValue ? (int?)query.MovementType.Value : null,
            query.SourceDocumentType
        };
        return ReadPageAsync<StockMovementRowResponse>(MovementSql, parameters, MovementOrder(query.SortBy), query.SortDirection,
            MovementStableOrder(query.SortBy), query.Page, query.PageSize, cancellationToken);
    }

    public Task<PagedReportResponse<CustomerAgingRowResponse>> GetCustomerAgingAsync(GetCustomerAgingQuery query, CancellationToken cancellationToken)
    {
        var parameters = new
        {
            AsOfDate = query.AsOfDate.ToDateTime(TimeOnly.MinValue),
            AsOfEndExclusive = new DateTimeOffset(query.AsOfDate.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc)),
            query.CustomerId
        };
        return ReadPageAsync<CustomerAgingRowResponse>(CustomerAgingSql, parameters, CustomerAgingOrder(query.SortBy),
            query.SortDirection, "CustomerId", query.Page, query.PageSize, cancellationToken);
    }

    private async Task<PagedReportResponse<T>> ReadPageAsync<T>(string cte, object filters, string orderColumn,
        string direction, string stableOrder, int page, int pageSize, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters(filters);
        parameters.Add("Offset", checked((long)(page - 1) * pageSize));
        parameters.Add("PageSize", pageSize);
        var ordering = $"{orderColumn} {(direction.Equals("desc", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC")}, {stableOrder}";
        var countSql = $"{cte} SELECT COUNT_BIG(1) FROM Filtered;";
        var pageSql = $"{cte} SELECT * FROM Filtered ORDER BY {ordering} OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        var total = await connection.ExecuteScalarAsync<long>(new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken));
        var rows = await connection.QueryAsync<T>(new CommandDefinition(pageSql, parameters, cancellationToken: cancellationToken));
        return new PagedReportResponse<T>(rows.AsList(), page, pageSize, total);
    }

    private static string SalesOrder(string key) => key.ToLowerInvariant() switch
    {
        "date" => "[Date]", "customer" => "CustomerName", "product" => "ProductName", "total" => "GrandTotal", _ => "[Date]"
    };
    private static string SalesStableOrder(string key) => key.ToLowerInvariant() == "date"
        ? "CustomerId, ProductId"
        : "[Date], CustomerId, ProductId";
    private static string ValuationOrder(string key) => key.ToLowerInvariant() switch
    {
        "warehouse" => "WarehouseName", "product" => "ProductName", "quantity" => "QtyOnHand", "cost" => "WeightedAverageCost", "value" => "InventoryValue", _ => "WarehouseName"
    };
    private static string LowStockOrder(string key) => key.ToLowerInvariant() switch
    {
        "warehouse" => "WarehouseName", "product" => "ProductName", "available" => "AvailableQuantity", "minimum" => "MinimumStockLevel", "reorder" => "ReorderQuantity", _ => "AvailableQuantity"
    };
    private static string MovementOrder(string key) => key.ToLowerInvariant() switch
    {
        "postedat" => "PostedAt", "warehouse" => "WarehouseName", "product" => "ProductName", "movementtype" => "MovementType", "sourcedocumenttype" => "SourceDocumentType", _ => "PostedAt"
    };
    private static string MovementStableOrder(string key) => key.Equals("postedAt", StringComparison.OrdinalIgnoreCase)
        ? "LedgerEntryId DESC"
        : "PostedAt DESC, LedgerEntryId DESC";
    private static string CustomerAgingOrder(string key) => key.ToLowerInvariant() switch
    {
        "current" => "[Current]", "days1to30" => "Days1To30", "days31to60" => "Days31To60",
        "days61to90" => "Days61To90", "days91plus" => "Days91Plus", "totaloutstanding" => "TotalOutstanding",
        _ => "CustomerName"
    };

    private sealed class SqlDateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
    {
        public override DateOnly Parse(object value) => value switch
        {
            DateTime dateTime => DateOnly.FromDateTime(dateTime),
            DateOnly dateOnly => dateOnly,
            _ => throw new DataException($"Cannot convert {value.GetType().Name} to DateOnly.")
        };

        public override void SetValue(IDbDataParameter parameter, DateOnly value)
        {
            parameter.DbType = DbType.Date;
            parameter.Value = value.ToDateTime(TimeOnly.MinValue);
        }
    }
}
