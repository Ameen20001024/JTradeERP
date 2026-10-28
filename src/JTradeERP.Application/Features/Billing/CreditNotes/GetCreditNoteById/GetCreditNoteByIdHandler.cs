using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Features.Billing.CreditNotes.CreateCreditNote;
using MediatR;

namespace JTradeERP.Application.Features.Billing.CreditNotes.GetCreditNoteById;

public sealed class GetCreditNoteByIdHandler(IApplicationDbContext db):IRequestHandler<GetCreditNoteByIdQuery,GetCreditNoteByIdResponse>
{
    public Task<GetCreditNoteByIdResponse> Handle(GetCreditNoteByIdQuery r,CancellationToken ct)
    {
        var x = db.CreditNotesWithLines.SingleOrDefault(x=>x.Id==r.Id)??throw new NotFoundException("Credit note not found.");
        
        return Task.FromResult(new GetCreditNoteByIdResponse(CreateCreditNoteHandler.Map(x)));
    }
}
