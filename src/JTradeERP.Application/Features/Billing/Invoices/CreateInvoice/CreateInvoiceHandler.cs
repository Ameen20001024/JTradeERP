using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Application.Features.Billing.Common;
using JTradeERP.Domain.Billing;
using MediatR;

namespace JTradeERP.Application.Features.Billing.Invoices.CreateInvoice;

public sealed class CreateInvoiceHandler
    (IApplicationDbContext db,IClock clock):IRequestHandler<CreateInvoiceCommand,CreateInvoiceResponse>
{
    public async Task<CreateInvoiceResponse> Handle(CreateInvoiceCommand r,CancellationToken ct)
    {
        BillingReferences.Customer(db,r.CustomerId);
        
        var w = db.Warehouses.SingleOrDefault(x=>x.Id==r.WarehouseId)
            ??throw new NotFoundException("Warehouse not found.");
        
        if(!w.IsActive)throw new ConflictException("Inactive warehouse cannot be invoiced.");
        
        SalesOrder? so=null;
        
        if(r.SourceSalesOrderId is{} soId)
        {
            so = db.SalesOrdersWithLines.SingleOrDefault(x=>x.Id==soId)
                ??throw new NotFoundException("Sales order not found.");
            
            if(so.Status is not
                (SalesOrderStatus.Posted or SalesOrderStatus.PartiallyFulfilled)||so.CustomerId!=r.CustomerId||so.WarehouseId!=r.WarehouseId)
                
                throw new ConflictException("Invoice must reference an active sales order with the same customer and warehouse.");
            
            foreach(var group in r.Lines.Where(x=>x.SourceSalesOrderLineId.HasValue).GroupBy(x=>x.SourceSalesOrderLineId!.Value))
            {
                var source=so.Lines.SingleOrDefault(x=>x.Id==group.Key)
                    ??throw new ConflictException("Invoice line does not belong to the referenced sales order.");
                
                if(group.Sum(x=>x.Quantity)>source.ReservedQuantity)
                    throw new ConflictException("Invoice quantity exceeds the sales order reservation.");
            }
            
            if(r.Lines.Any(x=>!x.SourceSalesOrderLineId.HasValue))
                throw new ConflictException("All lines on a sales-order invoice must reference sales-order lines.");
        }
        
        else if(r.Lines.Any(x=>x.SourceSalesOrderLineId.HasValue))
            throw new ConflictException("Source sales-order lines require a sales order.");
        
        var lines = r.Lines.Select(x=>BillingReferences.InvoiceLine(db,x));
        
        var invoice = new Invoice(
            r.CustomerId,
            r.WarehouseId,
            r.SourceSalesOrderId,
            r.InvoiceDate,
            lines,
            clock.UtcNow);
        
        db.Add(invoice);
        
        await db.SaveChangesAsync(ct);
        
        return new CreateInvoiceResponse(BillingResponseMapper.Map(invoice));
    }
}
