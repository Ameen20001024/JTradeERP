using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Application.Features.Billing.CreditNotes.CreateCreditNote;
using MediatR;

namespace JTradeERP.Application.Features.Billing.CreditNotes.CancelCreditNote;

public sealed class CancelCreditNoteHandler(IApplicationDbContext db,IClock clock):IRequestHandler<CancelCreditNoteCommand,CancelCreditNoteResponse>
{
    public async Task<CancelCreditNoteResponse> Handle(CancelCreditNoteCommand r,CancellationToken ct)
    {
        var n=db.CreditNotesWithLines.SingleOrDefault(x=>x.Id==r.Id)??throw new NotFoundException("Credit note not found.");
        
        n.Cancel(clock.UtcNow);
        
        await db.SaveChangesAsync(ct);
        
        return new CancelCreditNoteResponse(CreateCreditNoteHandler.Map(n));
    }
}
