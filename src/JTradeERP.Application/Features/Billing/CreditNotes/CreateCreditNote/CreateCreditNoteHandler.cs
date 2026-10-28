using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Billing;
using MediatR;

namespace JTradeERP.Application.Features.Billing.CreditNotes.CreateCreditNote;

public sealed class CreateCreditNoteHandler
    (IApplicationDbContext db,IClock clock):IRequestHandler<CreateCreditNoteCommand,CreateCreditNoteResponse>
{
    public async Task<CreateCreditNoteResponse> Handle(CreateCreditNoteCommand r,CancellationToken ct)
    {
        var invoice=db.InvoicesWithLines.SingleOrDefault(x=>x.Id==r.InvoiceId)??
            throw new NotFoundException("Invoice not found.");
        
        if(invoice.Status is not(InvoiceStatus.Posted or InvoiceStatus.PartiallyPaid or InvoiceStatus.Paid))
            throw new ConflictException("Credit notes require a posted invoice.");
        
        var lines=new List<CreditNoteLine>();
        foreach(var input in r.Lines)
        {
            var original = invoice.Lines.SingleOrDefault(x=>x.Id==input.InvoiceLineId)??
                throw new ConflictException("Credit note line must belong to the original invoice.");

            var previouslyCredited=db.CreditNotesWithLines
                .Where(x=>x.InvoiceId==invoice.Id&&x.Status==CreditNoteStatus.Posted)
                .SelectMany(x=>x.Lines)
                .Where(x=>x.InvoiceLineId==original.Id)
                .Sum(x=>x.Quantity);
            
            if(input.Quantity>original.Quantity-previouslyCredited)
                throw new ConflictException("Credit note quantity exceeds the uncredited invoice quantity.");
            lines.Add(new CreditNoteLine(
                original.Id,
                original.ProductId,
                input.Quantity,
                original.UnitPrice,
                original.TaxRateSnapshot));
        }
        var note = new CreditNote(
            invoice.Id,
            r.CreditNoteDate,
            lines,
            clock.UtcNow);
        
        db.Add(note);
        
        await db.SaveChangesAsync(ct);
        
        return Map(note);
    
    }
    
    internal static CreateCreditNoteResponse Map(CreditNote n) => new(
        n.Id,
        n.InvoiceId,
        n.CreditNoteDate,
        n.Status.ToString(),
        n.GrandTotal,
        n.Lines.Select(x=>new CreditNoteLineResponse(
            x.InvoiceLineId,
            x.ProductId,
            x.Quantity,
            x.UnitPrice,
            x.TaxRateSnapshot,
            x.TaxAmount,
            x.LineTotal))
        .ToArray());
}
