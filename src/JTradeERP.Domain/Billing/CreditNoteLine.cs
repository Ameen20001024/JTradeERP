namespace JTradeERP.Domain.Billing;

public sealed class CreditNoteLine
{
    private CreditNoteLine() { }

    public CreditNoteLine(Guid invoiceLineId, Guid productId, decimal quantity, decimal unitPrice, decimal taxRateSnapshot)
    {
        if (invoiceLineId == Guid.Empty || productId == Guid.Empty || quantity <= 0 || unitPrice < 0 || taxRateSnapshot is < 0 or > 100)
            throw new ArgumentException("Credit note line values are invalid.");
        Id = Guid.NewGuid(); InvoiceLineId = invoiceLineId; ProductId = productId; Quantity = quantity;
        UnitPrice = unitPrice; TaxRateSnapshot = taxRateSnapshot; Recalculate();
    }

    public Guid Id { get; private set; }
    public Guid CreditNoteId { get; private set; }
    public Guid InvoiceLineId { get; private set; }
    public Guid ProductId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TaxRateSnapshot { get; private set; }
    public decimal LineSubTotal { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal LineTotal { get; private set; }

    private void Recalculate()
    {
        LineSubTotal = BillingMath.Money(Quantity * UnitPrice);
        TaxAmount = BillingMath.Money(LineSubTotal * TaxRateSnapshot / 100m);
        LineTotal = LineSubTotal + TaxAmount;
    }
}
