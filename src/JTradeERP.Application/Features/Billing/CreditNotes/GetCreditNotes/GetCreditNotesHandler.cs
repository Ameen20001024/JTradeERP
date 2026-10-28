using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Features.Billing.CreditNotes.CreateCreditNote;
using MediatR;

namespace JTradeERP.Application.Features.Billing.CreditNotes.GetCreditNotes;

public sealed class GetCreditNotesHandler(IApplicationDbContext db):IRequestHandler<GetCreditNotesQuery,GetCreditNotesResponse>
{
    public Task<GetCreditNotesResponse> Handle(GetCreditNotesQuery r,CancellationToken ct)
    {
        var q = db.CreditNotesWithLines.AsQueryable();

        if(r.InvoiceId.HasValue)
            q = q.Where(x=>x.InvoiceId==r.InvoiceId);
        
        if(r.Status.HasValue)
            q = q.Where(x=>x.Status==r.Status);
        
        var count = q.Count();
        
        var items = q.OrderByDescending(x=>x.CreditNoteDate)
            .Skip((r.Page-1)*r.PageSize)
            .Take(r.PageSize)
            .ToList()
            .Select(CreateCreditNoteHandler.Map)
            .ToArray();
        
        return Task.FromResult(new GetCreditNotesResponse(
            items,
            count,
            r.Page,
            r.PageSize));
    }
}
