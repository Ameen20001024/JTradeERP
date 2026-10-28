namespace JTradeERP.Domain.Billing;

public sealed class Payment
{
    private readonly List<PaymentAllocation> allocations = [];
    private Payment() { }
    public Payment(DateOnly paymentDate, Guid paymentMethodId, decimal amount, string? referenceNumber,
        IEnumerable<PaymentAllocation> allocations, DateTimeOffset now)
    {
        if (paymentMethodId == Guid.Empty || amount <= 0) throw new ArgumentException("Payment method and positive amount are required.");
        Id = Guid.NewGuid(); PaymentDate = paymentDate; PaymentMethodId = paymentMethodId; Amount = BillingMath.Money(amount);
        ReferenceNumber = referenceNumber?.Trim(); Status = PaymentStatus.Posted; CreatedAt = now; ReplaceAllocations(allocations);
    }
    public Guid Id { get; private set; }
    public DateOnly PaymentDate { get; private set; }
    public Guid PaymentMethodId { get; private set; }
    public decimal Amount { get; private set; }
    public string? ReferenceNumber { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTimeOffset? ReversedAt { get; private set; }
    public Guid? ReversedByUserId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public IReadOnlyCollection<PaymentAllocation> Allocations => allocations.AsReadOnly();

    public void Reverse(DateTimeOffset now, Guid actor)
    {
        if (Status != PaymentStatus.Posted || actor == Guid.Empty) throw new InvalidOperationException("Only a posted payment can be reversed.");
        Status = PaymentStatus.Reversed; ReversedAt = now; ReversedByUserId = actor;
    }
    private void ReplaceAllocations(IEnumerable<PaymentAllocation> values)
    {
        var array = values.ToArray(); if (array.Length == 0) throw new ArgumentException("A payment requires at least one invoice allocation.");
        if (array.Sum(x => x.Amount) > Amount) throw new ArgumentException("Allocations cannot exceed payment amount.");
        if (array.GroupBy(x => x.InvoiceId).Any(x => x.Count() > 1)) throw new ArgumentException("Each invoice may appear once per payment.");
        allocations.AddRange(array);
    }
}
