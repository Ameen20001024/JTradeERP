namespace JTradeERP.Application.Features.Reports.SalesSummary;

public sealed record SalesSummaryRowResponse(DateOnly Date, Guid CustomerId, string CustomerName,
    Guid ProductId, string ProductCode, string ProductName, decimal SubTotal, decimal DiscountAmount,
    decimal NetAmount, decimal TaxAmount, decimal GrandTotal);
