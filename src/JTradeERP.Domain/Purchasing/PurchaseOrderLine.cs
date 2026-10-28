namespace JTradeERP.Domain.Purchasing;

public sealed class PurchaseOrderLine
{
    private PurchaseOrderLine() { }

    public PurchaseOrderLine(Guid productId, decimal orderedQuantity, decimal unitPurchaseCost,
        Guid taxCodeId, decimal taxRate, decimal discountAmount)
    {
        if (productId == Guid.Empty || taxCodeId == Guid.Empty)
            throw new ArgumentException("Product and tax code are required.");
        if (orderedQuantity <= 0 || unitPurchaseCost < 0 || taxRate is < 0 or > 100 || discountAmount < 0)
            throw new ArgumentOutOfRangeException(nameof(orderedQuantity), "Purchase order line values are invalid.");

        Id = Guid.NewGuid();
        ProductId = productId;
        OrderedQuantity = orderedQuantity;
        UnitPurchaseCost = unitPurchaseCost;
        TaxCodeId = taxCodeId;
        TaxRate = taxRate;
        DiscountAmount = discountAmount;
        Recalculate();
    }

    public Guid Id { get; private set; }
    public Guid PurchaseOrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public decimal OrderedQuantity { get; private set; }
    public decimal ReceivedQuantity { get; private set; }
    public decimal UnitPurchaseCost { get; private set; }
    public Guid TaxCodeId { get; private set; }
    public decimal TaxRate { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal LineSubTotal { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal LineTotal { get; private set; }

    internal void ApplyReceipt(decimal quantity)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        ReceivedQuantity += quantity;
    }

    internal void ReverseReceipt(decimal quantity)
    {
        if (quantity <= 0 || ReceivedQuantity < quantity)
            throw new InvalidOperationException("Receipt reversal exceeds the purchase order line's posted quantity.");
        ReceivedQuantity -= quantity;
    }

    private void Recalculate()
    {
        LineSubTotal = decimal.Round(OrderedQuantity * UnitPurchaseCost, 2, MidpointRounding.AwayFromZero);
        if (DiscountAmount > LineSubTotal)
            throw new ArgumentOutOfRangeException(nameof(DiscountAmount), "Discount cannot exceed line subtotal.");
        TaxAmount = decimal.Round((LineSubTotal - DiscountAmount) * TaxRate / 100m, 2, MidpointRounding.AwayFromZero);
        LineTotal = LineSubTotal - DiscountAmount + TaxAmount;
    }
}
