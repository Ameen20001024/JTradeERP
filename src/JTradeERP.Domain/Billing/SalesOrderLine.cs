namespace JTradeERP.Domain.Billing;

public sealed class SalesOrderLine
{
    private SalesOrderLine() { }

    public SalesOrderLine(Guid productId, decimal orderedQuantity, decimal unitPrice, decimal discountAmount,
        Guid taxCodeId, decimal taxRate, Guid? sourceQuotationLineId = null)
    {
        if (productId == Guid.Empty || taxCodeId == Guid.Empty || orderedQuantity <= 0 || unitPrice < 0 || discountAmount < 0 || taxRate is < 0 or > 100)
            throw new ArgumentException("Sales order line values are invalid.");
        Id = Guid.NewGuid(); ProductId = productId; OrderedQuantity = orderedQuantity; UnitPrice = unitPrice;
        DiscountAmount = discountAmount; TaxCodeId = taxCodeId; TaxRate = taxRate; SourceQuotationLineId = sourceQuotationLineId;
        Recalculate();
    }

    public Guid Id { get; private set; }
    public Guid SalesOrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public decimal OrderedQuantity { get; private set; }
    public decimal ReservedQuantity { get; private set; }
    public decimal FulfilledQuantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public Guid TaxCodeId { get; private set; }
    public decimal TaxRate { get; private set; }
    public decimal LineSubTotal { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal LineTotal { get; private set; }
    public Guid? SourceQuotationLineId { get; private set; }

    internal void Reserve(decimal quantity)
    {
        if (quantity <= 0 || ReservedQuantity + FulfilledQuantity + quantity > OrderedQuantity)
            throw new InvalidOperationException("Reservation exceeds the unfulfilled sales order quantity.");
        ReservedQuantity += quantity;
    }

    internal void Release(decimal quantity)
    {
        if (quantity < 0 || quantity > ReservedQuantity) throw new InvalidOperationException("Reservation release exceeds the remaining reserved quantity.");
        ReservedQuantity -= quantity;
    }

    internal void Fulfill(decimal quantity)
    {
        if (quantity <= 0 || quantity > ReservedQuantity) throw new InvalidOperationException("Invoice quantity exceeds the sales order reservation.");
        ReservedQuantity -= quantity; FulfilledQuantity += quantity;
    }

    internal void RestoreAfterInvoiceCancellation(decimal quantity, bool restoreReservation)
    {
        if (quantity <= 0 || quantity > FulfilledQuantity) throw new InvalidOperationException("Invoice reversal exceeds fulfilled quantity.");
        FulfilledQuantity -= quantity;
        if (restoreReservation) ReservedQuantity += quantity;
    }

    private void Recalculate()
    {
        LineSubTotal = BillingMath.Money(OrderedQuantity * UnitPrice);
        if (DiscountAmount > LineSubTotal) throw new ArgumentException("Discount cannot exceed line subtotal.");
        TaxAmount = BillingMath.Money((LineSubTotal - DiscountAmount) * TaxRate / 100m);
        LineTotal = LineSubTotal - DiscountAmount + TaxAmount;
    }
}
