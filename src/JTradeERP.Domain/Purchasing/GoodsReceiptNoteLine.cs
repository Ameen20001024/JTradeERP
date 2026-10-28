namespace JTradeERP.Domain.Purchasing;

public sealed class GoodsReceiptNoteLine
{
    private GoodsReceiptNoteLine() { }

    public GoodsReceiptNoteLine(Guid productId, Guid? purchaseOrderLineId, decimal receivedQuantity,
        decimal unitCost, Guid taxCodeId, decimal taxRate)
    {
        if (productId == Guid.Empty || taxCodeId == Guid.Empty)
            throw new ArgumentException("Product and tax code are required.");
        if (receivedQuantity <= 0 || unitCost < 0 || taxRate is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(receivedQuantity), "Goods receipt line values are invalid.");

        Id = Guid.NewGuid();
        ProductId = productId;
        PurchaseOrderLineId = purchaseOrderLineId;
        ReceivedQuantity = receivedQuantity;
        UnitCost = unitCost;
        TaxCodeId = taxCodeId;
        TaxRate = taxRate;
        LineSubTotal = decimal.Round(receivedQuantity * unitCost, 2, MidpointRounding.AwayFromZero);
        TaxAmount = decimal.Round(LineSubTotal * taxRate / 100m, 2, MidpointRounding.AwayFromZero);
        LineTotal = LineSubTotal + TaxAmount;
    }

    public Guid Id { get; private set; }
    public Guid GoodsReceiptNoteId { get; private set; }
    public Guid? PurchaseOrderLineId { get; private set; }
    public Guid ProductId { get; private set; }
    public decimal ReceivedQuantity { get; private set; }
    public decimal UnitCost { get; private set; }
    public Guid TaxCodeId { get; private set; }
    public decimal TaxRate { get; private set; }
    public decimal LineSubTotal { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal LineTotal { get; private set; }
}
