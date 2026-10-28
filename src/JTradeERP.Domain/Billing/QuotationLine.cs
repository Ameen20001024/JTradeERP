namespace JTradeERP.Domain.Billing;

public sealed class QuotationLine
{
    private QuotationLine() { }

    public QuotationLine(Guid productId, decimal quantity, decimal unitPrice, decimal discountAmount, Guid taxCodeId, decimal taxRate)
    {
        if (productId == Guid.Empty || taxCodeId == Guid.Empty || quantity <= 0 || unitPrice < 0 || discountAmount < 0 || taxRate is < 0 or > 100)
            throw new ArgumentException("Quotation line values are invalid.");
        Id = Guid.NewGuid(); ProductId = productId; Quantity = quantity; UnitPrice = unitPrice;
        DiscountAmount = discountAmount; TaxCodeId = taxCodeId; TaxRate = taxRate; Recalculate();
    }

    public Guid Id { get; private set; }
    public Guid QuotationId { get; private set; }
    public Guid ProductId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public Guid TaxCodeId { get; private set; }
    public decimal TaxRate { get; private set; }
    public decimal LineSubTotal { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal LineTotal { get; private set; }

    private void Recalculate()
    {
        LineSubTotal = BillingMath.Money(Quantity * UnitPrice);
        if (DiscountAmount > LineSubTotal) throw new ArgumentException("Discount cannot exceed line subtotal.");
        TaxAmount = BillingMath.Money((LineSubTotal - DiscountAmount) * TaxRate / 100m);
        LineTotal = LineSubTotal - DiscountAmount + TaxAmount;
    }
}
