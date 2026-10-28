using JTradeERP.Application.Features.Billing.CreditNotes.CreateCreditNote;

namespace JTradeERP.Application.Features.Billing.CreditNotes.GetCreditNotes;

public sealed record GetCreditNotesResponse(
    IReadOnlyList<CreateCreditNoteResponse> Items,
    int TotalCount,
    int Page,
    int PageSize);
