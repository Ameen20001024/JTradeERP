namespace JTradeERP.Application.Features.Billing.CreditNotes.CreateCreditNote;

public sealed record CreditNoteLineResponse(
    Guid InvoiceLineId,
    Guid ProductId,
    decimal Quantity,
    decimal UnitPrice,
    decimal TaxRateSnapshot,
    decimal TaxAmount,
    decimal LineTotal);

public sealed record CreateCreditNoteResponse(
    Guid Id,
    Guid InvoiceId,
    DateOnly CreditNoteDate,
    string Status,
    decimal GrandTotal,
    IReadOnlyList<CreditNoteLineResponse> Lines);
