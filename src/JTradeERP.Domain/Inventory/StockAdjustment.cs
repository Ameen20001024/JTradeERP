namespace JTradeERP.Domain.Inventory;

public sealed class StockAdjustment
{
    private StockAdjustment() { }

    public StockAdjustment(Guid productId, Guid warehouseId, decimal quantityDelta, string reasonCode)
    {
        if (productId == Guid.Empty || warehouseId == Guid.Empty || quantityDelta == 0 || string.IsNullOrWhiteSpace(reasonCode))
            throw new ArgumentException("Adjustment product, warehouse, non-zero quantity and reason are required.");
        Id = Guid.NewGuid();
        ProductId = productId;
        WarehouseId = warehouseId;
        QuantityDelta = quantityDelta;
        ReasonCode = reasonCode.Trim();
        Status = InventoryDocumentStatus.Draft;
    }

    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public decimal QuantityDelta { get; private set; }
    public decimal? UnitCost { get; private set; }
    public string ReasonCode { get; private set; } = null!;
    public InventoryDocumentStatus Status { get; private set; }
    public DateTimeOffset? PostedAt { get; private set; }
    public Guid? PostedByUserId { get; private set; }
    public DateTimeOffset? ReversedAt { get; private set; }
    public Guid? ReversedByUserId { get; private set; }

    public void Update(Guid productId, Guid warehouseId, decimal quantityDelta, string reasonCode)
    {
        if (Status != InventoryDocumentStatus.Draft)
            throw new InvalidOperationException("Only draft adjustments can be edited.");
        if (productId == Guid.Empty || warehouseId == Guid.Empty || quantityDelta == 0 || string.IsNullOrWhiteSpace(reasonCode))
            throw new ArgumentException("Adjustment product, warehouse, non-zero quantity and reason are required.");
        ProductId = productId;
        WarehouseId = warehouseId;
        QuantityDelta = quantityDelta;
        ReasonCode = reasonCode.Trim();
    }

    public void Post(decimal? unitCost, DateTimeOffset postedAt, Guid postedByUserId)
    {
        if (Status != InventoryDocumentStatus.Draft)
            throw new InvalidOperationException("Only draft adjustments can be posted.");
        if (QuantityDelta > 0 && (!unitCost.HasValue || unitCost.Value < 0))
            throw new ArgumentException("Positive adjustments require a non-negative valuation unit cost.");
        UnitCost = unitCost;
        PostedAt = postedAt;
        PostedByUserId = postedByUserId;
        Status = InventoryDocumentStatus.Posted;
    }

    public void MarkReversed(DateTimeOffset reversedAt, Guid reversedByUserId)
    {
        if (Status != InventoryDocumentStatus.Posted)
            throw new InvalidOperationException("Only posted adjustments can be reversed.");
        ReversedAt = reversedAt;
        ReversedByUserId = reversedByUserId;
        Status = InventoryDocumentStatus.Reversed;
    }
}
