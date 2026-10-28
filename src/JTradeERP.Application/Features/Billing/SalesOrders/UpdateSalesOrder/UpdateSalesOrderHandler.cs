using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Application.Features.Billing.Common;
using JTradeERP.Domain.Billing;
using MediatR;

namespace JTradeERP.Application.Features.Billing.SalesOrders.UpdateSalesOrder;

public sealed class UpdateSalesOrderHandler(IApplicationDbContext db,IClock clock):IRequestHandler<UpdateSalesOrderCommand,UpdateSalesOrderResponse>
{
    public async Task<UpdateSalesOrderResponse> Handle(UpdateSalesOrderCommand r,CancellationToken ct)
    {
        var order=db.SalesOrdersWithLines.SingleOrDefault(x=>x.Id==r.Id)??throw new NotFoundException("Sales order not found.");
        
        BillingReferences.Customer(db,r.CustomerId);
        
        var w = db.Warehouses.SingleOrDefault(x=>x.Id==r.WarehouseId)??throw new NotFoundException("Warehouse not found.");
        
        if(!w.IsActive)throw new ConflictException("Inactive warehouse cannot be used.");
        
        var lines = r.Lines.Select(x=>
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
        
        order.UpdateDraft(
            r.CustomerId,
            r.WarehouseId,
            r.OrderDate,
            r.DeliveryDate,
            r.DeliveryAddress,
            lines,
            clock.UtcNow);

        await db.SaveChangesAsync(ct);
        return new UpdateSalesOrderResponse(BillingResponseMapper.Map(order));
    }
}
