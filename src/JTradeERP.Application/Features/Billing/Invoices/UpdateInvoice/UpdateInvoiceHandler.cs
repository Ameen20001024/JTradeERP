using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Application.Features.Billing.Common;
using JTradeERP.Domain.Billing;
using MediatR;

namespace JTradeERP.Application.Features.Billing.Invoices.UpdateInvoice;

public sealed class UpdateInvoiceHandler
    (IApplicationDbContext db,IClock clock):IRequestHandler<UpdateInvoiceCommand,UpdateInvoiceResponse>
{
    public async Task<UpdateInvoiceResponse> Handle(UpdateInvoiceCommand r,CancellationToken ct)
    {
        var invoice=db.InvoicesWithLines.SingleOrDefault(x=>x.Id==r.Id)
            ??throw new NotFoundException("Invoice not found.");
        
        BillingReferences.Customer(db,r.CustomerId);
        
        var warehouse=db.Warehouses.SingleOrDefault(x=>x.Id==r.WarehouseId)
            ??throw new NotFoundException("Warehouse not found.");
        
        if(!warehouse.IsActive)throw new ConflictException("Inactive warehouse cannot be used.");
        
        if(invoice.SourceSalesOrderId is{} sourceId)
        {
            var so=db.SalesOrdersWithLines.SingleOrDefault(x=>x.Id==sourceId)
                ??throw new NotFoundException("Sales order not found.");
            
            if(so.CustomerId!=r.CustomerId||so.WarehouseId!=r.WarehouseId)
                throw new ConflictException("Invoice customer and warehouse must match its sales order.");
            
            if(r.Lines.Any(x=>x.SourceSalesOrderLineId is null||so.Lines.All(y=>y.Id!=x.SourceSalesOrderLineId)))
                throw new ConflictException("Every linked invoice line must preserve its sales order line.");
        
        }
        
        var lines = r.Lines.Select(x=>BillingReferences.InvoiceLine(db,x));
        
        invoice.UpdateDraft(
            r.CustomerId,
            r.WarehouseId,
            r.InvoiceDate,
            lines,
            clock.UtcNow);
        
        await db.SaveChangesAsync(ct);
        
        return new(BillingResponseMapper.Map(invoice));
    }
}
