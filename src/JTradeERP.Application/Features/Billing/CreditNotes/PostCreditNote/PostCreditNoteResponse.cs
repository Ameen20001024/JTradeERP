using JTradeERP.Application.Features.Billing.CreditNotes.CreateCreditNote;

namespace JTradeERP.Application.Features.Billing.CreditNotes.PostCreditNote;

public sealed record PostCreditNoteResponse(CreateCreditNoteResponse CreditNote);
