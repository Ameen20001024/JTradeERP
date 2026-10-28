using MediatR;

namespace JTradeERP.Application.Features.Billing.CreditNotes.PostCreditNote;

public sealed record PostCreditNoteCommand(Guid Id):IRequest<PostCreditNoteResponse>;
