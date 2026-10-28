using System.Diagnostics;
using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Inventory;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Application.Features.Billing.Common;
using JTradeERP.Domain.Billing;
using JTradeERP.Domain.Inventory;
using MediatR;

namespace JTradeERP.Application.Features.Billing.Invoices.CancelInvoice;

public sealed class CancelInvoiceHandler(
    IApplicationDbContext db,
    IStockPostingService stock,
    ICurrentUserService actor,
    IClock clock) : IRequestHandler<CancelInvoiceCommand, CancelInvoiceResponse>
{
    public async Task<CancelInvoiceResponse> Handle(CancelInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = db.InvoicesWithLines.SingleOrDefault(x => x.Id == request.Id)
            ?? throw new NotFoundException("Invoice not found.");
        
        if (invoice.Status == InvoiceStatus.Cancelled)
            return new(BillingResponseMapper.Map(invoice));

        var userId = actor.UserId ?? throw new AuthenticationFailureException();
        
        await using var transaction = await db.BeginTransactionAsync(cancellationToken);
        
        var originalEntries = db.StockLedgerEntries.Where(x =>
            x.SourceDocumentType == "Invoice" && x.SourceDocumentId == invoice.Id &&
            x.MovementType == StockMovementType.Issue && x.Direction == StockMovementDirection.Out).ToList();
        
        var order = invoice.SourceSalesOrderId is { } orderId
            ? db.SalesOrdersWithLines.SingleOrDefault(x => x.Id == orderId)
                ?? throw new NotFoundException("Sales order not found.")
            : null;

        foreach (var entry in originalEntries)
        {
            stock.Reverse(entry, invoice.Id, "InvoiceCancellation", userId,
                Activity.Current?.Id ?? Guid.NewGuid().ToString());
            
            if (order is null) continue;

            var invoiceLine = invoice.Lines.Single(x => x.Id == entry.SourceLineId);
            
            var orderLineId = invoiceLine.SourceSalesOrderLineId
                ?? throw new ConflictException("Sales-order invoice line is missing its source line.");
            
            var orderLine = order.Lines.Single(x => x.Id == orderLineId);
            
            var restoreReservation = order.Status != SalesOrderStatus.Cancelled;
            
            if (restoreReservation) 
                stock.Reserve(orderLine.ProductId, order.WarehouseId, invoiceLine.Quantity);
            
            order.ReverseFulfillment(
                orderLine.Id,
                invoiceLine.Quantity,
                restoreReservation,
                clock.UtcNow);
        }

        invoice.Cancel(clock.UtcNow, userId);

        SecurityAuditWriter.Add(
            db,
            userId,
            "InvoiceCancelled",
            "Invoice",
            invoice.Id.ToString(),
            invoice.InvoiceNumber,
            "Cancelled; stock effects reversed");


        await db.SaveChangesAsync(cancellationToken);
        
        await transaction.CommitAsync(cancellationToken);
        
        return new CancelInvoiceResponse(BillingResponseMapper.Map(invoice));
    }
}
