using MediatR;

namespace JTradeERP.Application.Features.Billing.CreditNotes.GetCreditNoteById;

public sealed record GetCreditNoteByIdQuery(Guid Id):IRequest<GetCreditNoteByIdResponse>;
