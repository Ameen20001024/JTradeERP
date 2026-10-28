using JTradeERP.Domain.Billing;

namespace JTradeERP.Application.Features.Billing.Common;

public sealed record BillingLineInput(
    Guid ProductId,
    decimal Quantity,
    decimal UnitPrice,
    decimal DiscountAmount,
    Guid TaxCodeId);


public sealed record SalesOrderLineInput(
    Guid ProductId,
    decimal OrderedQuantity,
    decimal UnitPrice,
    decimal DiscountAmount,
    Guid TaxCodeId,
    Guid? SourceQuotationLineId = null);


public sealed record BillingLineResponse(
    Guid Id,
    Guid ProductId,
    decimal Quantity,
    decimal UnitPrice,
    decimal DiscountAmount,
    Guid TaxCodeId,
    decimal TaxRate,
    decimal TaxAmount,
    decimal LineTotal);


public sealed record QuotationDocumentResponse(
    Guid Id,
    Guid CustomerId,
    DateOnly QuotationDate,
    DateOnly ValidityDate,
    string Status,
    decimal SubTotal,
    decimal DiscountAmount,
    decimal TaxAmount,
    decimal GrandTotal,
    IReadOnlyList<BillingLineResponse> Lines);


public sealed record SalesOrderDocumentResponse(
    Guid Id,
    Guid? SourceQuotationId,
    Guid CustomerId,
    Guid WarehouseId,
    DateOnly OrderDate,
    DateOnly? DeliveryDate,
    string? DeliveryAddress,
    string Status,
    decimal GrandTotal,
    IReadOnlyList<SalesOrderLineResponse> Lines);


public sealed record SalesOrderLineResponse(
    Guid Id,
    Guid ProductId,
    decimal OrderedQuantity,
    decimal ReservedQuantity,
    decimal FulfilledQuantity,
    decimal UnitPrice,
    Guid TaxCodeId,
    decimal TaxRate,
    Guid? SourceQuotationLineId);


public sealed record InvoiceLineInput(
    Guid ProductId,
    Guid? SourceSalesOrderLineId,
    decimal Quantity,
    decimal UnitPrice,
    decimal DiscountAmount,
    Guid TaxCodeId);


public sealed record InvoiceLineResponse(
    Guid Id,
    Guid ProductId,
    Guid? SourceSalesOrderLineId,
    decimal Quantity,
    decimal UnitPrice,
    decimal DiscountAmount,
    Guid TaxCodeId,
    decimal TaxRateSnapshot,
    decimal TaxAmountSnapshot,
    decimal LineTotal);


public sealed record InvoiceDocumentResponse(
    Guid Id,
    string? InvoiceNumber,
    Guid? SourceSalesOrderId,
    Guid CustomerId,
    Guid WarehouseId,
    DateOnly InvoiceDate,
    string Status,
    string? CompanyTrnSnapshot,
    string? CustomerTrnSnapshot,
    decimal GrandTotal,
    decimal PaidAmount,
    decimal OutstandingAmount,
    IReadOnlyList<InvoiceLineResponse> Lines);


public static class BillingResponseMapper
{
    public static QuotationDocumentResponse Map(Quotation x) =>
        new(
            x.Id,
            x.CustomerId,
            x.QuotationDate,
            x.ValidityDate,
            x.Status.ToString(),
            x.SubTotal,
            x.DiscountAmount,
            x.TaxAmount,
            x.GrandTotal,
            x.Lines.Select(l => new BillingLineResponse(
                l.Id,
                l.ProductId,
                l.Quantity,
                l.UnitPrice,
                l.DiscountAmount,
                l.TaxCodeId,
                l.TaxRate,
                l.TaxAmount,
                l.LineTotal))
            .ToArray());


    public static SalesOrderDocumentResponse Map(SalesOrder x) => new(
        x.Id,
        x.SourceQuotationId,
        x.CustomerId,
        x.WarehouseId,
        x.OrderDate,
        x.DeliveryDate,
        x.DeliveryAddress,
        x.Status.ToString(),
        x.GrandTotal,
        x.Lines.Select(l => new SalesOrderLineResponse(
            l.Id,
            l.ProductId,
            l.OrderedQuantity,
            l.ReservedQuantity,
            l.FulfilledQuantity,
            l.UnitPrice,
            l.TaxCodeId,
            l.TaxRate,
            l.SourceQuotationLineId))
        .ToArray());
    
    
    public static InvoiceDocumentResponse Map(Invoice x) => new(
        x.Id,
        x.InvoiceNumber,
        x.SourceSalesOrderId,
        x.CustomerId,
        x.WarehouseId,
        x.InvoiceDate,
        x.Status.ToString(),
        x.CompanyTrnSnapshot,
        x.CustomerTrnSnapshot,
        x.GrandTotal,
        x.PaidAmount,
        x.OutstandingAmount,
        x.Lines.Select(l => new InvoiceLineResponse(
            l.Id,
            l.ProductId,
            l.SourceSalesOrderLineId,
            l.Quantity,
            l.UnitPrice,
            l.DiscountAmount,
            l.TaxCodeId,
            l.TaxRateSnapshot,
            l.TaxAmountSnapshot,
            l.LineTotal))
        .ToArray());
}
