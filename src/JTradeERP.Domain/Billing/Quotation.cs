namespace JTradeERP.Domain.Billing;

public sealed class Quotation
{
    private readonly List<QuotationLine> lines = [];
    private Quotation() { }

    public Quotation(Guid customerId, DateOnly quotationDate, DateOnly validityDate, IEnumerable<QuotationLine> lines, DateTimeOffset now)
    {
        if (customerId == Guid.Empty || validityDate < quotationDate) throw new ArgumentException("Quotation customer or dates are invalid.");
        Id = Guid.NewGuid(); CustomerId = customerId; QuotationDate = quotationDate; ValidityDate = validityDate;
        Status = QuotationStatus.Draft; CreatedAt = UpdatedAt = now; ReplaceLines(lines);
    }

    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public DateOnly QuotationDate { get; private set; }
    public DateOnly ValidityDate { get; private set; }
    public QuotationStatus Status { get; private set; }
    public decimal SubTotal { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal GrandTotal { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public IReadOnlyCollection<QuotationLine> Lines => lines.AsReadOnly();

    public void UpdateDraft(Guid customerId, DateOnly date, DateOnly validUntil, IEnumerable<QuotationLine> replacement, DateTimeOffset now)
    {
        if (Status != QuotationStatus.Draft) throw new InvalidOperationException("Only draft quotations can be edited.");
        if (customerId == Guid.Empty || validUntil < date) throw new ArgumentException("Quotation customer or dates are invalid.");
        CustomerId = customerId; QuotationDate = date; ValidityDate = validUntil; UpdatedAt = now; ReplaceLines(replacement);
    }

    public void Send(DateTimeOffset now) => Transition(QuotationStatus.Draft, QuotationStatus.Sent, now);
    public void Accept(DateTimeOffset now) => Transition(QuotationStatus.Sent, QuotationStatus.Accepted, now);
    public void Reject(DateTimeOffset now) => Transition(QuotationStatus.Sent, QuotationStatus.Rejected, now);
    public void Expire(DateOnly today, DateTimeOffset now)
    {
        if (Status != QuotationStatus.Sent || ValidityDate >= today) throw new InvalidOperationException("Only sent quotations past their validity date can expire.");
        Status = QuotationStatus.Expired; UpdatedAt = now;
    }
    public void Cancel(DateTimeOffset now)
    {
        if (Status is QuotationStatus.Cancelled or QuotationStatus.Rejected or QuotationStatus.Expired)
            throw new InvalidOperationException("This quotation cannot be cancelled in its current state.");
        Status = QuotationStatus.Cancelled; UpdatedAt = now;
    }

    private void Transition(QuotationStatus expected, QuotationStatus next, DateTimeOffset now)
    {
        if (Status != expected) throw new InvalidOperationException($"Quotation must be {expected} before it can become {next}.");
        Status = next; UpdatedAt = now;
    }

    private void ReplaceLines(IEnumerable<QuotationLine> replacement)
    {
        var values = replacement.ToArray();
        if (values.Length == 0) throw new ArgumentException("A quotation requires at least one line.");
        lines.Clear(); lines.AddRange(values); SubTotal = lines.Sum(x => x.LineSubTotal);
        DiscountAmount = lines.Sum(x => x.DiscountAmount); TaxAmount = lines.Sum(x => x.TaxAmount);
        GrandTotal = SubTotal - DiscountAmount + TaxAmount;
    }
}
