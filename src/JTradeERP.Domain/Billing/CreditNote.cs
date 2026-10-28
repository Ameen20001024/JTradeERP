namespace JTradeERP.Domain.Billing;

public sealed class CreditNote
{
    private readonly List<CreditNoteLine> lines = [];
    private CreditNote() { }

    public CreditNote(Guid invoiceId, DateOnly date, IEnumerable<CreditNoteLine> lines, DateTimeOffset now)
    {
        if (invoiceId == Guid.Empty) throw new ArgumentException("Original invoice is required.");
        Id = Guid.NewGuid(); InvoiceId = invoiceId; CreditNoteDate = date; Status = CreditNoteStatus.Draft;
        CreatedAt = UpdatedAt = now; ReplaceLines(lines);
    }

    public Guid Id { get; private set; }
    public Guid InvoiceId { get; private set; }
    public DateOnly CreditNoteDate { get; private set; }
    public CreditNoteStatus Status { get; private set; }
    public decimal SubTotal { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal GrandTotal { get; private set; }
    public DateTimeOffset? PostedAt { get; private set; }
    public Guid? PostedByUserId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public IReadOnlyCollection<CreditNoteLine> Lines => lines.AsReadOnly();

    public void UpdateDraft(DateOnly date, IEnumerable<CreditNoteLine> replacement, DateTimeOffset now)
    {
        if (Status != CreditNoteStatus.Draft) throw new InvalidOperationException("Only draft credit notes can be edited.");
        CreditNoteDate = date; UpdatedAt = now; ReplaceLines(replacement);
    }
    public void Post(DateTimeOffset now, Guid actor)
    {
        if (Status != CreditNoteStatus.Draft || actor == Guid.Empty) throw new InvalidOperationException("Only draft credit notes can be posted.");
        Status = CreditNoteStatus.Posted; PostedAt = now; PostedByUserId = actor; UpdatedAt = now;
    }
    public void Cancel(DateTimeOffset now)
    {
        if (Status != CreditNoteStatus.Draft) throw new InvalidOperationException("Only a draft credit note can be cancelled.");
        Status = CreditNoteStatus.Cancelled; UpdatedAt = now;
    }
    private void ReplaceLines(IEnumerable<CreditNoteLine> replacement)
    {
        var values = replacement.ToArray(); if (values.Length == 0) throw new ArgumentException("A credit note requires at least one line.");
        lines.Clear(); lines.AddRange(values); SubTotal = lines.Sum(x => x.LineSubTotal); TaxAmount = lines.Sum(x => x.TaxAmount); GrandTotal = SubTotal + TaxAmount;
    }
}
