using MediatR;
using JTradeERP.Application.Features.Billing.CreditNotes.CreateCreditNote;

namespace JTradeERP.Application.Features.Billing.CreditNotes.UpdateCreditNote;

public sealed record UpdateCreditNoteCommand
    (Guid Id,DateOnly CreditNoteDate,IReadOnlyCollection<CreditNoteLineInput> Lines):IRequest<UpdateCreditNoteResponse>;
