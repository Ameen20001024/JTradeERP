using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Application.Features.Billing.CreditNotes.CreateCreditNote;
using JTradeERP.Domain.Billing;
using MediatR;

namespace JTradeERP.Application.Features.Billing.CreditNotes.UpdateCreditNote;

public sealed class UpdateCreditNoteHandler
    (IApplicationDbContext db,IClock clock):IRequestHandler<UpdateCreditNoteCommand,UpdateCreditNoteResponse>
{
    public async Task<UpdateCreditNoteResponse> Handle(UpdateCreditNoteCommand r,CancellationToken ct)
    {
        var note = db.CreditNotesWithLines.SingleOrDefault(x=>x.Id==r.Id)
            ??throw new NotFoundException("Credit note not found.");
        
        var invoice = db.InvoicesWithLines.SingleOrDefault(x=>x.Id==note.InvoiceId)
            ??throw new NotFoundException("Invoice not found.");
        
        var lines = new List<CreditNoteLine>();
        
        foreach(var input in r.Lines)
        {
            var invLine = invoice.Lines.SingleOrDefault(x=>x.Id==input.InvoiceLineId)
                ??throw new ConflictException("Credit note line must belong to the invoice.");
            
            var credited = db.CreditNotesWithLines
                .Where(x=>x.InvoiceId==invoice.Id&&x.Id!=note.Id&&x.Status==CreditNoteStatus.Posted)
                .SelectMany(x=>x.Lines)
                .Where(x=>x.InvoiceLineId==invLine.Id)
                .Sum(x=>x.Quantity);
            
            if(input.Quantity>invLine.Quantity-credited)
                throw new ConflictException("Credit quantity exceeds the remaining invoice quantity.");
            
            lines.Add(new CreditNoteLine(
                invLine.Id,
                invLine.ProductId,
                input.Quantity,
                invLine.UnitPrice,
                invLine.TaxRateSnapshot));
        
        }
        
        note.UpdateDraft(r.CreditNoteDate,lines,clock.UtcNow);
        
        await db.SaveChangesAsync(ct);
        
        return new(CreateCreditNoteHandler.Map(note));
    }
}
