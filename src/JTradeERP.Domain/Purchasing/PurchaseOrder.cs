namespace JTradeERP.Domain.Purchasing;

public sealed class PurchaseOrder
{
    private readonly List<PurchaseOrderLine> lines = [];
    private PurchaseOrder() { }

    public PurchaseOrder(Guid supplierId, Guid warehouseId, DateOnly orderDate, DateOnly? expectedDate,
        Guid createdByUserId, IEnumerable<PurchaseOrderLine> lines, DateTimeOffset now)
    {
        if (supplierId == Guid.Empty || warehouseId == Guid.Empty || createdByUserId == Guid.Empty)
            throw new ArgumentException("Supplier, warehouse, and creator are required.");
        if (expectedDate.HasValue && expectedDate.Value < orderDate)
            throw new ArgumentException("Expected date cannot precede order date.");

        Id = Guid.NewGuid();
        SupplierId = supplierId;
        WarehouseId = warehouseId;
        OrderDate = orderDate;
        ExpectedDate = expectedDate;
        Status = PurchaseOrderStatus.Draft;
        CreatedAt = UpdatedAt = now;
        CreatedByUserId = createdByUserId;
        ReplaceLines(lines);
    }

    public Guid Id { get; private set; }
    public Guid SupplierId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public DateOnly OrderDate { get; private set; }
    public DateOnly? ExpectedDate { get; private set; }
    public PurchaseOrderStatus Status { get; private set; }
    public decimal SubTotal { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal GrandTotal { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public Guid? UpdatedByUserId { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public IReadOnlyCollection<PurchaseOrderLine> Lines => lines.AsReadOnly();

    public void UpdateDraft(Guid supplierId, Guid warehouseId, DateOnly orderDate, DateOnly? expectedDate,
        Guid updatedByUserId, IEnumerable<PurchaseOrderLine> replacementLines, DateTimeOffset now)
    {
        if (Status != PurchaseOrderStatus.Draft)
            throw new InvalidOperationException("Only draft purchase orders can be edited.");
        if (supplierId == Guid.Empty || warehouseId == Guid.Empty || updatedByUserId == Guid.Empty)
            throw new ArgumentException("Supplier, warehouse, and updater are required.");
        if (expectedDate.HasValue && expectedDate.Value < orderDate)
            throw new ArgumentException("Expected date cannot precede order date.");

        SupplierId = supplierId;
        WarehouseId = warehouseId;
        OrderDate = orderDate;
        ExpectedDate = expectedDate;
        UpdatedByUserId = updatedByUserId;
        UpdatedAt = now;
        ReplaceLines(replacementLines);
    }

    public void ApplyReceipt(IReadOnlyDictionary<Guid, decimal> quantities, DateTimeOffset now)
    {
        if (Status == PurchaseOrderStatus.Cancelled)
            throw new InvalidOperationException("A cancelled purchase order cannot receive goods.");
        foreach (var (lineId, quantity) in quantities)
            (lines.SingleOrDefault(x => x.Id == lineId)
                ?? throw new InvalidOperationException("A receipt references a line outside this purchase order.")).ApplyReceipt(quantity);
        RecalculateReceiptStatus();
        UpdatedAt = now;
    }

    public void ReverseReceipt(IReadOnlyDictionary<Guid, decimal> quantities, DateTimeOffset now)
    {
        if (Status is PurchaseOrderStatus.Cancelled or PurchaseOrderStatus.Draft)
            throw new InvalidOperationException("The purchase order cannot reverse receipt progress in its current state.");
        foreach (var (lineId, quantity) in quantities)
            (lines.SingleOrDefault(x => x.Id == lineId)
                ?? throw new InvalidOperationException("A receipt references a line outside this purchase order.")).ReverseReceipt(quantity);
        RecalculateReceiptStatus();
        UpdatedAt = now;
    }

    private void ReplaceLines(IEnumerable<PurchaseOrderLine> replacementLines)
    {
        var values = replacementLines.ToArray();
        if (values.Length == 0) throw new ArgumentException("A purchase order requires at least one line.");
        lines.Clear();
        lines.AddRange(values);
        RecalculateTotals();
    }

    private void RecalculateTotals()
    {
        SubTotal = lines.Sum(x => x.LineSubTotal);
        DiscountAmount = lines.Sum(x => x.DiscountAmount);
        TaxAmount = lines.Sum(x => x.TaxAmount);
        GrandTotal = SubTotal - DiscountAmount + TaxAmount;
    }

    private void RecalculateReceiptStatus()
    {
        if (lines.All(x => x.ReceivedQuantity == 0)) Status = PurchaseOrderStatus.Open;
        else if (lines.All(x => x.ReceivedQuantity >= x.OrderedQuantity)) Status = PurchaseOrderStatus.FullyReceived;
        else Status = PurchaseOrderStatus.PartiallyReceived;
    }
}
