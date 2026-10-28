using MediatR;

namespace JTradeERP.Application.Features.Billing.CreditNotes.CancelCreditNote;

public sealed record CancelCreditNoteCommand(Guid Id):IRequest<CancelCreditNoteResponse>;
