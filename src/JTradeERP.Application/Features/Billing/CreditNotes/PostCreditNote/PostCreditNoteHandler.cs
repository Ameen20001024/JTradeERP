using System.Diagnostics;
using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Inventory;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Application.Features.Billing.CreditNotes.CreateCreditNote;
using JTradeERP.Domain.Billing;
using JTradeERP.Domain.Inventory;
using MediatR;

namespace JTradeERP.Application.Features.Billing.CreditNotes.PostCreditNote;

public sealed class PostCreditNoteHandler(
    IApplicationDbContext db,
    IStockPostingService stock,
    ICurrentUserService actor,
    IClock clock) : IRequestHandler<PostCreditNoteCommand, PostCreditNoteResponse>
{
    public async Task<PostCreditNoteResponse> Handle(PostCreditNoteCommand request, CancellationToken cancellationToken)
    {
        var creditNote = db.CreditNotesWithLines.SingleOrDefault(x => x.Id == request.Id)
            ?? throw new NotFoundException("Credit note not found.");

        if (creditNote.Status == CreditNoteStatus.Posted)
            return new(CreateCreditNoteHandler.Map(creditNote));
        
        if (creditNote.Status != CreditNoteStatus.Draft)
            throw new ConflictException("Only a draft credit note can be posted.");

        var invoice = db.InvoicesWithLines.SingleOrDefault(x => x.Id == creditNote.InvoiceId)
            ?? throw new NotFoundException("Invoice not found.");
        
        if (invoice.Status is not (InvoiceStatus.Posted or InvoiceStatus.PartiallyPaid or InvoiceStatus.Paid))
            throw new ConflictException("Credit notes require a posted invoice.");
        
        var userId = actor.UserId ?? throw new AuthenticationFailureException();
        
        await using var transaction = await db.BeginTransactionAsync(cancellationToken);

        foreach (var line in creditNote.Lines)
        {
            var invoiceLine = invoice.Lines.Single(x => x.Id == line.InvoiceLineId);
        
            var alreadyCredited = db.CreditNotesWithLines
                .Where(x => x.InvoiceId == invoice.Id && x.Status == CreditNoteStatus.Posted)
                .SelectMany(x => x.Lines)
                .Where(x => x.InvoiceLineId == invoiceLine.Id)
                .Sum(x => x.Quantity);
            
            if (alreadyCredited + line.Quantity > invoiceLine.Quantity)
                throw new ConflictException("Cumulative credit quantity exceeds the original invoice quantity.");

            var originalIssue = db.StockLedgerEntries.SingleOrDefault(x =>
                x.SourceDocumentType == "Invoice" && x.SourceDocumentId == invoice.Id &&
                x.SourceLineId == invoiceLine.Id && x.MovementType == StockMovementType.Issue &&
                x.Direction == StockMovementDirection.Out)
                ?? throw new ConflictException("The original invoice stock issue was not found.");

            stock.Receive(
                invoiceLine.ProductId,
                invoice.WarehouseId,
                line.Quantity,
                originalIssue.UnitCost,
                "CreditNote",
                creditNote.Id,
                line.Id,
                StockMovementType.Receipt,
                userId,
                Activity.Current?.Id ?? Guid.NewGuid().ToString());
        }

        creditNote.Post(clock.UtcNow, userId);

        invoice.Touch(clock.UtcNow);
        
        SecurityAuditWriter.Add(db, userId, "CreditNotePosted", "CreditNote", creditNote.Id.ToString(),
            CreditNoteStatus.Draft.ToString(), CreditNoteStatus.Posted.ToString());
        
        await db.SaveChangesAsync(cancellationToken);
        
        await transaction.CommitAsync(cancellationToken);
        
        return new PostCreditNoteResponse(CreateCreditNoteHandler.Map(creditNote));
    }
}
