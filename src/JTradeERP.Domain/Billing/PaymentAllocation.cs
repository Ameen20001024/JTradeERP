namespace JTradeERP.Domain.Billing;

public sealed class PaymentAllocation
{
    private PaymentAllocation() { }
    public PaymentAllocation(Guid invoiceId, decimal amount)
    {
        if (invoiceId == Guid.Empty || amount <= 0) throw new ArgumentException("A positive allocation and invoice are required.");
        Id = Guid.NewGuid(); InvoiceId = invoiceId; Amount = BillingMath.Money(amount);
    }
    public Guid Id { get; private set; }
    public Guid PaymentId { get; private set; }
    public Guid InvoiceId { get; private set; }
    public decimal Amount { get; private set; }
}
