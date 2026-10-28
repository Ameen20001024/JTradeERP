using MediatR;

namespace JTradeERP.Application.Features.Billing.CreditNotes.CreateCreditNote;

public sealed record CreditNoteLineInput(
    Guid InvoiceLineId,
    decimal Quantity);

public sealed record CreateCreditNoteCommand(
    Guid InvoiceId,
    DateOnly CreditNoteDate,
    IReadOnlyCollection<CreditNoteLineInput> Lines):IRequest<CreateCreditNoteResponse>;
