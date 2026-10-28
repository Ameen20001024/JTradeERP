namespace JTradeERP.Domain.Inventory;

public sealed class OpeningBalance
{
    private OpeningBalance() { }

    public OpeningBalance(Guid sourceDocumentId,
        Guid productId,
        Guid warehouseId, 
        decimal quantity, 
        decimal unitCost, 
        DateTimeOffset postedAt, 
        Guid postedByUserId)

    {
        if (sourceDocumentId == Guid.Empty || productId == Guid.Empty || warehouseId == Guid.Empty || postedByUserId == Guid.Empty)
            throw new ArgumentException("Opening-balance identity fields are required.");
        
        if (quantity <= 0 || unitCost < 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Opening balance requires positive quantity and a non-negative unit cost.");

        Id = Guid.NewGuid();
        SourceDocumentId = sourceDocumentId;
        ProductId = productId;
        WarehouseId = warehouseId;
        Quantity = quantity;
        UnitCost = unitCost;
        Status = InventoryDocumentStatus.Posted;
        PostedAt = postedAt;
        PostedByUserId = postedByUserId;
    }

    public Guid Id { get; private set; }
    
    public Guid SourceDocumentId { get; private set; }
    
    public Guid ProductId { get; private set; }
    
    public Guid WarehouseId { get; private set; }
    
    public decimal Quantity { get; private set; }
    
    public decimal UnitCost { get; private set; }
    
    public InventoryDocumentStatus Status { get; private set; }
    
    public DateTimeOffset PostedAt { get; private set; }
    
    public Guid PostedByUserId { get; private set; }
    
    public DateTimeOffset? ReversedAt { get; private set; }
    
    public Guid? ReversedByUserId { get; private set; }

    public void MarkReversed(DateTimeOffset reversedAt, Guid reversedByUserId)
    {
        if (Status != InventoryDocumentStatus.Posted)
            throw new InvalidOperationException("Only posted opening balances can be reversed.");
        ReversedAt = reversedAt;
        ReversedByUserId = reversedByUserId;
        Status = InventoryDocumentStatus.Reversed;
    }
}
