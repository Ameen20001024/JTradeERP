namespace JTradeERP.Domain.Purchasing;

public sealed class GoodsReceiptNote
{
    private readonly List<GoodsReceiptNoteLine> lines = [];
    private GoodsReceiptNote() { }

    public GoodsReceiptNote(Guid? purchaseOrderId, Guid supplierId, Guid warehouseId, DateOnly receiptDate,
        IEnumerable<GoodsReceiptNoteLine> lines, DateTimeOffset now)
    {
        if (supplierId == Guid.Empty || warehouseId == Guid.Empty)
            throw new ArgumentException("Supplier and warehouse are required.");
        Id = Guid.NewGuid();
        PurchaseOrderId = purchaseOrderId;
        SupplierId = supplierId;
        WarehouseId = warehouseId;
        ReceiptDate = receiptDate;
        Status = GoodsReceiptNoteStatus.Draft;
        CreatedAt = UpdatedAt = now;
        ReplaceLines(lines);
    }

    public Guid Id { get; private set; }
    public Guid? PurchaseOrderId { get; private set; }
    public Guid SupplierId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public DateOnly ReceiptDate { get; private set; }
    public GoodsReceiptNoteStatus Status { get; private set; }
    public DateTimeOffset? PostedAt { get; private set; }
    public Guid? PostedByUserId { get; private set; }
    public DateTimeOffset? CancelledAt { get; private set; }
    public Guid? CancelledByUserId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public IReadOnlyCollection<GoodsReceiptNoteLine> Lines => lines.AsReadOnly();

    public void UpdateDraft(Guid? purchaseOrderId, Guid supplierId, Guid warehouseId, DateOnly receiptDate,
        IEnumerable<GoodsReceiptNoteLine> replacementLines, DateTimeOffset now)
    {
        if (Status != GoodsReceiptNoteStatus.Draft)
            throw new InvalidOperationException("Only draft goods receipt notes can be edited.");
        if (supplierId == Guid.Empty || warehouseId == Guid.Empty)
            throw new ArgumentException("Supplier and warehouse are required.");
        PurchaseOrderId = purchaseOrderId;
        SupplierId = supplierId;
        WarehouseId = warehouseId;
        ReceiptDate = receiptDate;
        UpdatedAt = now;
        ReplaceLines(replacementLines);
    }

    public void Post(DateTimeOffset now, Guid actorUserId)
    {
        if (Status != GoodsReceiptNoteStatus.Draft)
            throw new InvalidOperationException("Only draft goods receipt notes can be posted.");
        if (actorUserId == Guid.Empty || lines.Count == 0)
            throw new ArgumentException("A posting actor and at least one receipt line are required.");
        Status = GoodsReceiptNoteStatus.Posted;
        PostedAt = now;
        PostedByUserId = actorUserId;
        UpdatedAt = now;
    }

    public void Cancel(DateTimeOffset now, Guid actorUserId)
    {
        if (Status != GoodsReceiptNoteStatus.Posted)
            throw new InvalidOperationException("Only posted goods receipt notes can be cancelled.");
        Status = GoodsReceiptNoteStatus.Cancelled;
        CancelledAt = now;
        CancelledByUserId = actorUserId;
        UpdatedAt = now;
    }

    private void ReplaceLines(IEnumerable<GoodsReceiptNoteLine> replacementLines)
    {
        var values = replacementLines.ToArray();
        if (values.Length == 0) throw new ArgumentException("A goods receipt note requires at least one line.");
        lines.Clear();
        lines.AddRange(values);
    }
}
