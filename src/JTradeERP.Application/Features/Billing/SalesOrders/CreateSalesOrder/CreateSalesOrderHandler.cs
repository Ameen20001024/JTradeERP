using JTradeERP.Application.Common.Exceptions;using JTradeERP.Application.Common.Persistence;using JTradeERP.Application.Common.Security;using JTradeERP.Application.Features.Billing.Common;using JTradeERP.Domain.Billing;using MediatR;
namespace JTradeERP.Application.Features.Billing.SalesOrders.CreateSalesOrder;
public sealed class CreateSalesOrderHandler(IApplicationDbContext db,IClock clock):IRequestHandler<CreateSalesOrderCommand,CreateSalesOrderResponse>
{
    public async Task<CreateSalesOrderResponse> Handle(CreateSalesOrderCommand r,CancellationToken ct)
    {
        BillingReferences.Customer(db,r.CustomerId);
        
        var w = db.Warehouses.SingleOrDefault(x=>x.Id==r.WarehouseId)??throw new NotFoundException("Warehouse not found.");
        
        if(!w.IsActive)throw new ConflictException("Inactive warehouse cannot be used.");
        
        Quotation? q=null;
        
        if(r.SourceQuotationId is{} qid)
        {
            q=db.QuotationsWithLines.SingleOrDefault(x=>x.Id==qid)??throw new NotFoundException("Quotation not found.");
            
            if(q.Status!=QuotationStatus.Accepted||q.CustomerId!=r.CustomerId)
                
                throw new ConflictException("Only an accepted quotation for the same customer can be converted.");
            
            if(r.Lines.Any(x=>x.SourceQuotationLineId is null||q.Lines.All(l=>l.Id!=x.SourceQuotationLineId)))
                
                throw new ConflictException("Converted sales order lines must preserve source quotation lines.");
        }
        
        var lines=r.Lines.Select(x => 
        {
            var p = db.Products.SingleOrDefault(y=>y.Id==x.ProductId)??throw new NotFoundException("Product not found.");
            
            var t = db.TaxCodes.SingleOrDefault(y=>y.Id==x.TaxCodeId)??throw new NotFoundException("Tax code not found.");
            
            if(!p.IsActive||!t.IsActive)
                throw new ConflictException("Inactive product or tax code cannot be used.");
            
            return new SalesOrderLine(
                x.ProductId,
                x.OrderedQuantity,
                x.UnitPrice,
                x.DiscountAmount,
                x.TaxCodeId,
                t.Rate,
                x.SourceQuotationLineId);
        });
        
        var entity=new SalesOrder(
            r.CustomerId,
            r.WarehouseId,
            r.SourceQuotationId,
            r.OrderDate,
            r.DeliveryDate,
            r.DeliveryAddress,
            lines,
            clock.UtcNow);
        
        db.Add(entity);
        
        await db.SaveChangesAsync(ct);
        
        return new(BillingResponseMapper.Map(entity));
    }
}
