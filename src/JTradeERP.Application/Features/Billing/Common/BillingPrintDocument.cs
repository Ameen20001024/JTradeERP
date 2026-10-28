using System.Net;
using JTradeERP.Application.Features.Billing.Common;

namespace JTradeERP.Application.Features.Billing.Common;

internal static class BillingPrintDocument
{
    public static string Quotation(QuotationDocumentResponse document) => Render(
        "Quotation",
        document.Id,
        document.Status,
        document.SubTotal,
        document.DiscountAmount,
        document.TaxAmount,
        document.GrandTotal,
        null,
        document.Lines.Select(line => (
            line.ProductId,
            line.Quantity,
            line.UnitPrice,
            line.TaxRate,
            line.TaxAmount,
            line.LineTotal)));

    public static string Invoice(InvoiceDocumentResponse document) => Render(
        "Tax Invoice",
        document.Id, 
        document.Status,
        document.Lines.Sum(line => line.Quantity * line.UnitPrice),
        document.Lines.Sum(line => line.DiscountAmount),
        document.Lines.Sum(line => line.TaxAmountSnapshot), 
        document.GrandTotal,
        $"<p><strong>Invoice number:</strong> {E(document.InvoiceNumber ?? "Draft")}</p>" +
        $"<p><strong>Supplier TRN:</strong> {E(document.CompanyTrnSnapshot ?? string.Empty)}</p>" +
        $"<p><strong>Customer TRN:</strong> {E(document.CustomerTrnSnapshot ?? string.Empty)}</p>",

        document.Lines.Select(line => (
            line.ProductId,
            line.Quantity,
            line.UnitPrice,
            line.TaxRateSnapshot,
            line.TaxAmountSnapshot,
            line.LineTotal)));

    private static string Render(
        string title,
        Guid id,
        string status,
        decimal subtotal,
        decimal discount,
        decimal tax,
        decimal total,
        string? extra,
        IEnumerable<(Guid ProductId, decimal Quantity, decimal UnitPrice, decimal TaxRate, decimal TaxAmount, decimal LineTotal)> lines)
    
    {
        var rows = string.Join("", lines.Select(line => $"<tr><td>{line.ProductId}</td><td>{line.Quantity:N2}</td><td>{line.UnitPrice:N2}</td><td>" +
        $"{line.TaxRate:N2}%</td><td>{line.TaxAmount:N2}</td><td>{line.LineTotal:N2}</td></tr>"));
        
        return "<!doctype html><html lang=\"en\"><meta charset=\"utf-8\"><title>" + E(title) +
            "</title><style>body{font:14px Arial;margin:32px}table{border-collapse:collapse;width:100%}th,td{border:1px solid #888;padding:8px;text-align:right}th:first-child,td:first-child{text-align:left}.totals{margin-left:auto;width:280px}</style><body><h1>" + E(title) +
            "</h1><p><strong>Document ID:</strong> " + id + "</p><p><strong>Status:</strong> " + E(status) + "</p>" + (extra ?? "") +
            "<table><thead><tr><th>Product ID</th><th>Quantity</th><th>Unit price</th><th>VAT rate</th><th>VAT</th><th>Total</th></tr></thead><tbody>" + rows +
            "</tbody></table><table class=\"totals\"><tr><th>Subtotal</th><td>" + subtotal.ToString("N2") + "</td></tr><tr><th>Discount</th><td>" + discount.ToString("N2") +
            "</td></tr><tr><th>VAT</th><td>" + tax.ToString("N2") + "</td></tr><tr><th>Total</th><td>" + total.ToString("N2") + "</td></tr></table><script>window.print()</script></body></html>";
    }

    private static string E(string value) => WebUtility.HtmlEncode(value);
}
