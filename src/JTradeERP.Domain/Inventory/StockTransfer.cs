namespace JTradeERP.Domain.Inventory;

public sealed class StockTransfer
{
    private StockTransfer() { }

    public StockTransfer(Guid sourceWarehouseId, Guid destinationWarehouseId)
    {
        if (sourceWarehouseId == Guid.Empty || destinationWarehouseId == Guid.Empty || sourceWarehouseId == destinationWarehouseId)
            throw new ArgumentException("Transfer requires two different warehouses.");
        Id = Guid.NewGuid();
        SourceWarehouseId = sourceWarehouseId;
        DestinationWarehouseId = destinationWarehouseId;
        Status = InventoryDocumentStatus.Draft;
    }

    public Guid Id { get; private set; }
    public Guid SourceWarehouseId { get; private set; }
    public Guid DestinationWarehouseId { get; private set; }
    public InventoryDocumentStatus Status { get; private set; }
    public DateTimeOffset? PostedAt { get; private set; }
    public Guid? PostedByUserId { get; private set; }
    public ICollection<StockTransferLine> Lines { get; private set; } = new List<StockTransferLine>();

    public void Update(Guid sourceWarehouseId, Guid destinationWarehouseId, IReadOnlyCollection<(Guid ProductId, decimal Quantity)> lines)
    {
        if (Status != InventoryDocumentStatus.Draft)
            throw new InvalidOperationException("Only draft transfers can be edited.");
        if (sourceWarehouseId == Guid.Empty || destinationWarehouseId == Guid.Empty || sourceWarehouseId == destinationWarehouseId)
            throw new ArgumentException("Transfer requires two different warehouses.");
        if (lines.Count == 0 || lines.Any(x => x.ProductId == Guid.Empty || x.Quantity <= 0))
            throw new ArgumentException("Transfer requires valid product lines with positive quantities.");

        SourceWarehouseId = sourceWarehouseId;
        DestinationWarehouseId = destinationWarehouseId;
        Lines.Clear();
        foreach (var line in lines)
            Lines.Add(new StockTransferLine(Id, line.ProductId, line.Quantity));
    }

    public void Post(DateTimeOffset postedAt, Guid postedByUserId, int lineCount)
    {
        if (Status != InventoryDocumentStatus.Draft || lineCount <= 0)
            throw new InvalidOperationException("Only a non-empty draft transfer can be posted.");
        PostedAt = postedAt;
        PostedByUserId = postedByUserId;
        Status = InventoryDocumentStatus.Posted;
    }
}

public sealed class StockTransferLine
{
    private StockTransferLine() { }
    internal StockTransferLine(Guid stockTransferId, Guid productId, decimal quantity)
    {
        if (stockTransferId == Guid.Empty || productId == Guid.Empty || quantity <= 0)
            throw new ArgumentException("Transfer line is invalid.");
        Id = Guid.NewGuid();
        StockTransferId = stockTransferId;
        ProductId = productId;
        Quantity = quantity;
    }

    public Guid Id { get; private set; }
    public Guid StockTransferId { get; private set; }
    public Guid ProductId { get; private set; }
    public decimal Quantity { get; private set; }
}
