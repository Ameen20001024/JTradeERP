namespace JTradeERP.Domain.Billing;

public sealed class InvoiceLine
{
    private InvoiceLine() { }

    public InvoiceLine(Guid productId, Guid? sourceSalesOrderLineId, decimal quantity, decimal unitPrice,
        decimal discountAmount, Guid taxCodeId, decimal taxRate)
    {
        if (productId == Guid.Empty || taxCodeId == Guid.Empty || quantity <= 0 || unitPrice < 0 || discountAmount < 0 || taxRate is < 0 or > 100)
            throw new ArgumentException("Invoice line values are invalid.");
        Id = Guid.NewGuid(); ProductId = productId; SourceSalesOrderLineId = sourceSalesOrderLineId;
        Quantity = quantity; UnitPrice = unitPrice; DiscountAmount = discountAmount; TaxCodeId = taxCodeId;
        TaxRateSnapshot = taxRate; Recalculate();
    }

    public Guid Id { get; private set; }
    public Guid InvoiceId { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid? SourceSalesOrderLineId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public Guid TaxCodeId { get; private set; }
    public decimal TaxRateSnapshot { get; private set; }
    public decimal LineSubTotal { get; private set; }
    public decimal TaxAmountSnapshot { get; private set; }
    public decimal LineTotal { get; private set; }

    internal void SnapshotTax(decimal rate)
    {
        if (rate is < 0 or > 100) throw new ArgumentOutOfRangeException(nameof(rate));
        TaxRateSnapshot = rate; Recalculate();
    }

    private void Recalculate()
    {
        LineSubTotal = BillingMath.Money(Quantity * UnitPrice);
        if (DiscountAmount > LineSubTotal) throw new ArgumentException("Discount cannot exceed line subtotal.");
        TaxAmountSnapshot = BillingMath.Money((LineSubTotal - DiscountAmount) * TaxRateSnapshot / 100m);
        LineTotal = LineSubTotal - DiscountAmount + TaxAmountSnapshot;
    }
}
