using MediatR;
using JTradeERP.Domain.Billing;

namespace JTradeERP.Application.Features.Billing.CreditNotes.GetCreditNotes;

public sealed record GetCreditNotesQuery(
    Guid? InvoiceId=null,
    CreditNoteStatus? Status=null,
    int Page=1,
    int PageSize=50):IRequest<GetCreditNotesResponse>;
