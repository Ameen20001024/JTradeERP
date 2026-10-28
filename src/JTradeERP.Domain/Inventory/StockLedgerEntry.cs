namespace JTradeERP.Domain.Inventory;

public sealed class StockLedgerEntry
{
    private StockLedgerEntry() { }

    public StockLedgerEntry(
        Guid productId,
        Guid warehouseId,
        StockMovementType movementType,
        StockMovementDirection direction,
        decimal quantity,
        decimal unitCost,
        string sourceDocumentType,
        Guid sourceDocumentId,
        Guid? sourceLineId,
        Guid? reversalOfLedgerEntryId,
        DateTimeOffset postedAt,
        Guid postedByUserId,
        string correlationId)
    {

        if (productId == Guid.Empty || warehouseId == Guid.Empty || sourceDocumentId == Guid.Empty || postedByUserId == Guid.Empty)
            throw new ArgumentException("Stock ledger identity fields are required.");
        
        if (quantity <= 0 || unitCost < 0 || string.IsNullOrWhiteSpace(sourceDocumentType) || string.IsNullOrWhiteSpace(correlationId))
            throw new ArgumentException("Stock ledger movement fields are invalid.");
        
        if ((movementType == StockMovementType.Reversal) != reversalOfLedgerEntryId.HasValue)
            throw new ArgumentException("A reversal must reference exactly one original ledger entry.");

        Id = Guid.NewGuid();
        ProductId = productId;
        WarehouseId = warehouseId;
        MovementType = movementType;
        Direction = direction;
        Quantity = quantity;
        UnitCost = unitCost;
        SourceDocumentType = sourceDocumentType.Trim();
        SourceDocumentId = sourceDocumentId;
        SourceLineId = sourceLineId;
        ReversalOfLedgerEntryId = reversalOfLedgerEntryId;
        PostedAt = postedAt;
        PostedByUserId = postedByUserId;
        CorrelationId = correlationId;
    }

    public Guid Id { get; private set; }
    
    public Guid ProductId { get; private set; }
    
    public Guid WarehouseId { get; private set; }
    
    public StockMovementType MovementType { get; private set; }
    
    public StockMovementDirection Direction { get; private set; }
    
    public decimal Quantity { get; private set; }
    
    public decimal UnitCost { get; private set; }
    
    public string SourceDocumentType { get; private set; } = null!;
    
    public Guid SourceDocumentId { get; private set; }
    
    public Guid? SourceLineId { get; private set; }
    
    public Guid? ReversalOfLedgerEntryId { get; private set; }
    
    public DateTimeOffset PostedAt { get; private set; }
    
    public Guid PostedByUserId { get; private set; }
    
    public string CorrelationId { get; private set; } = null!;
}
