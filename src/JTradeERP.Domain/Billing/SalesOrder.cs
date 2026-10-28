namespace JTradeERP.Domain.Billing;

public sealed class SalesOrder
{
    private readonly List<SalesOrderLine> lines = [];
    private SalesOrder() { }

    public SalesOrder(Guid customerId, Guid warehouseId, Guid? sourceQuotationId, DateOnly orderDate,
        DateOnly? deliveryDate, string? deliveryAddress, IEnumerable<SalesOrderLine> lines, DateTimeOffset now)
    {
        if (customerId == Guid.Empty || warehouseId == Guid.Empty || deliveryDate < orderDate)
            throw new ArgumentException("Sales order customer, warehouse or dates are invalid.");
        Id = Guid.NewGuid(); CustomerId = customerId; WarehouseId = warehouseId; SourceQuotationId = sourceQuotationId;
        OrderDate = orderDate; DeliveryDate = deliveryDate; DeliveryAddress = deliveryAddress?.Trim();
        Status = SalesOrderStatus.Draft; CreatedAt = UpdatedAt = now; ReplaceLines(lines);
    }

    public Guid Id { get; private set; }
    public Guid? SourceQuotationId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public DateOnly OrderDate { get; private set; }
    public DateOnly? DeliveryDate { get; private set; }
    public string? DeliveryAddress { get; private set; }
    public SalesOrderStatus Status { get; private set; }
    public decimal SubTotal { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal GrandTotal { get; private set; }
    public DateTimeOffset? PostedAt { get; private set; }
    public Guid? PostedByUserId { get; private set; }
    public DateTimeOffset? CancelledAt { get; private set; }
    public Guid? CancelledByUserId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public IReadOnlyCollection<SalesOrderLine> Lines => lines.AsReadOnly();

    public void UpdateDraft(Guid customerId, Guid warehouseId, DateOnly orderDate, DateOnly? deliveryDate,
        string? deliveryAddress, IEnumerable<SalesOrderLine> replacement, DateTimeOffset now)
    {
        if (Status != SalesOrderStatus.Draft) throw new InvalidOperationException("Only draft sales orders can be edited.");
        if (customerId == Guid.Empty || warehouseId == Guid.Empty || deliveryDate < orderDate)
            throw new ArgumentException("Sales order customer, warehouse or dates are invalid.");
        CustomerId = customerId; WarehouseId = warehouseId; OrderDate = orderDate; DeliveryDate = deliveryDate;
        DeliveryAddress = deliveryAddress?.Trim(); UpdatedAt = now; ReplaceLines(replacement);
    }

    public void Post(DateTimeOffset now, Guid actor)
    {
        if (Status != SalesOrderStatus.Draft || actor == Guid.Empty) throw new InvalidOperationException("Only a draft sales order can be posted.");
        foreach (var line in lines) line.Reserve(line.OrderedQuantity);
        Status = SalesOrderStatus.Posted; PostedAt = now; PostedByUserId = actor; UpdatedAt = now;
    }

    public void Cancel(DateTimeOffset now, Guid actor)
    {
        if (Status is SalesOrderStatus.Cancelled or SalesOrderStatus.Fulfilled || actor == Guid.Empty)
            throw new InvalidOperationException("This sales order cannot be cancelled in its current state.");
        foreach (var line in lines) line.Release(line.ReservedQuantity);
        Status = SalesOrderStatus.Cancelled; CancelledAt = now; CancelledByUserId = actor; UpdatedAt = now;
    }

    public void Fulfill(Guid lineId, decimal quantity, DateTimeOffset now)
    {
        if (Status is not (SalesOrderStatus.Posted or SalesOrderStatus.PartiallyFulfilled))
            throw new InvalidOperationException("Only an active posted sales order can be fulfilled.");
        FindLine(lineId).Fulfill(quantity); UpdateFulfillmentStatus(now);
    }

    public void ReverseFulfillment(Guid lineId, decimal quantity, bool restoreReservation, DateTimeOffset now)
    {
        FindLine(lineId).RestoreAfterInvoiceCancellation(quantity, restoreReservation);
        if (Status != SalesOrderStatus.Cancelled) UpdateFulfillmentStatus(now);
        else UpdatedAt = now;
    }

    private SalesOrderLine FindLine(Guid id) => lines.SingleOrDefault(x => x.Id == id)
        ?? throw new InvalidOperationException("Invoice line does not belong to this sales order.");

    private void UpdateFulfillmentStatus(DateTimeOffset now)
    {
        Status = lines.All(x => x.FulfilledQuantity == x.OrderedQuantity)
            ? SalesOrderStatus.Fulfilled
            : lines.Any(x => x.FulfilledQuantity > 0) ? SalesOrderStatus.PartiallyFulfilled : SalesOrderStatus.Posted;
        UpdatedAt = now;
    }

    private void ReplaceLines(IEnumerable<SalesOrderLine> replacement)
    {
        var values = replacement.ToArray(); if (values.Length == 0) throw new ArgumentException("A sales order requires at least one line.");
        lines.Clear(); lines.AddRange(values); SubTotal = lines.Sum(x => x.LineSubTotal);
        DiscountAmount = lines.Sum(x => x.DiscountAmount); TaxAmount = lines.Sum(x => x.TaxAmount); GrandTotal = SubTotal - DiscountAmount + TaxAmount;
    }
}
