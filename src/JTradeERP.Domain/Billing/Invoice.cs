namespace JTradeERP.Domain.Billing;

public sealed class Invoice
{
    private readonly List<InvoiceLine> lines = [];
    private Invoice() { }

    public Invoice(Guid customerId, Guid warehouseId, Guid? sourceSalesOrderId, DateOnly invoiceDate,
        IEnumerable<InvoiceLine> lines, DateTimeOffset now)
    {
        if (customerId == Guid.Empty || warehouseId == Guid.Empty) throw new ArgumentException("Invoice customer and warehouse are required.");
        Id = Guid.NewGuid(); CustomerId = customerId; WarehouseId = warehouseId; SourceSalesOrderId = sourceSalesOrderId;
        InvoiceDate = invoiceDate; Status = InvoiceStatus.Draft; CreatedAt = UpdatedAt = now; ReplaceLines(lines);
    }

    public Guid Id { get; private set; }
    public string? InvoiceNumber { get; private set; }
    public Guid? SourceSalesOrderId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public DateOnly InvoiceDate { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public string? CompanyTrnSnapshot { get; private set; }
    public string? CustomerTrnSnapshot { get; private set; }
    public decimal SubTotal { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal GrandTotal { get; private set; }
    public decimal PaidAmount { get; private set; }
    public decimal OutstandingAmount { get; private set; }
    public DateTimeOffset? PostedAt { get; private set; }
    public Guid? PostedByUserId { get; private set; }
    public DateTimeOffset? CancelledAt { get; private set; }
    public Guid? CancelledByUserId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public IReadOnlyCollection<InvoiceLine> Lines => lines.AsReadOnly();

    public void UpdateDraft(Guid customerId, Guid warehouseId, DateOnly date, IEnumerable<InvoiceLine> replacement, DateTimeOffset now)
    {
        if (Status != InvoiceStatus.Draft) throw new InvalidOperationException("Only draft invoices can be edited.");
        if (customerId == Guid.Empty || warehouseId == Guid.Empty) throw new ArgumentException("Invoice customer and warehouse are required.");
        CustomerId = customerId; WarehouseId = warehouseId; InvoiceDate = date; UpdatedAt = now; ReplaceLines(replacement);
    }

    public void Post(string number, string? companyTrn, string? customerTrn, DateTimeOffset now, Guid actor)
    {
        if (Status != InvoiceStatus.Draft || actor == Guid.Empty || string.IsNullOrWhiteSpace(number))
            throw new InvalidOperationException("Only a draft invoice can be posted with a valid invoice number and actor.");
        InvoiceNumber = number; CompanyTrnSnapshot = companyTrn; CustomerTrnSnapshot = customerTrn;
        PostedAt = now; PostedByUserId = actor; Status = InvoiceStatus.Posted;
        OutstandingAmount = GrandTotal; UpdatedAt = now;
    }

    public void SnapshotTaxRates(IReadOnlyDictionary<Guid, decimal> rates)
    {
        if (Status != InvoiceStatus.Draft) throw new InvalidOperationException("Only a draft invoice can snapshot tax rates.");
        foreach (var line in lines)
        {
            if (!rates.TryGetValue(line.TaxCodeId, out var rate)) throw new InvalidOperationException("Invoice tax code was not found.");
            line.SnapshotTax(rate);
        }
        SubTotal = lines.Sum(x => x.LineSubTotal); DiscountAmount = lines.Sum(x => x.DiscountAmount);
        TaxAmount = lines.Sum(x => x.TaxAmountSnapshot); GrandTotal = SubTotal - DiscountAmount + TaxAmount;
    }

    public void ApplyPayment(decimal amount, DateTimeOffset now)
    {
        if (Status is not (InvoiceStatus.Posted or InvoiceStatus.PartiallyPaid) || amount <= 0 || amount > OutstandingAmount)
            throw new InvalidOperationException("Payment allocation exceeds invoice outstanding or invoice is not payable.");
        PaidAmount = BillingMath.Money(PaidAmount + amount); OutstandingAmount = BillingMath.Money(GrandTotal - PaidAmount);
        Status = OutstandingAmount == 0 ? InvoiceStatus.Paid : InvoiceStatus.PartiallyPaid; UpdatedAt = now;
    }

    public void ReversePayment(decimal amount, DateTimeOffset now)
    {
        if (Status is not (InvoiceStatus.Posted or InvoiceStatus.PartiallyPaid or InvoiceStatus.Paid) || amount <= 0 || amount > PaidAmount)
            throw new InvalidOperationException("Payment reversal exceeds invoice paid amount.");
        PaidAmount = BillingMath.Money(PaidAmount - amount); OutstandingAmount = BillingMath.Money(GrandTotal - PaidAmount);
        Status = PaidAmount == 0 ? InvoiceStatus.Posted : InvoiceStatus.PartiallyPaid; UpdatedAt = now;
    }

    public void Cancel(DateTimeOffset now, Guid actor)
    {
        if (Status is not (InvoiceStatus.Posted or InvoiceStatus.PartiallyPaid or InvoiceStatus.Paid) || PaidAmount != 0 || actor == Guid.Empty)
            throw new InvalidOperationException("Invoice must be posted with no active payment allocations before cancellation.");
        Status = InvoiceStatus.Cancelled; OutstandingAmount = 0; CancelledAt = now; CancelledByUserId = actor; UpdatedAt = now;
    }

    public void Touch(DateTimeOffset now)
    {
        if (Status == InvoiceStatus.Cancelled) throw new InvalidOperationException("Cancelled invoice is immutable.");
        UpdatedAt = now;
    }

    private void ReplaceLines(IEnumerable<InvoiceLine> replacement)
    {
        var values = replacement.ToArray(); if (values.Length == 0) throw new ArgumentException("An invoice requires at least one line.");
        lines.Clear(); lines.AddRange(values); SubTotal = lines.Sum(x => x.LineSubTotal);
        DiscountAmount = lines.Sum(x => x.DiscountAmount); TaxAmount = lines.Sum(x => x.TaxAmountSnapshot);
        GrandTotal = SubTotal - DiscountAmount + TaxAmount;
    }
}
