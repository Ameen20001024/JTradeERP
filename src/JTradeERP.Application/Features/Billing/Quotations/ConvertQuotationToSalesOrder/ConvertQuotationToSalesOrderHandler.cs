using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Application.Features.Billing.Common;
using JTradeERP.Domain.Billing;
using MediatR;

namespace JTradeERP.Application.Features.Billing.Quotations.ConvertQuotationToSalesOrder;

public sealed class ConvertQuotationToSalesOrderHandler(
    IApplicationDbContext db,
    ICurrentUserService actor,
    IClock clock) : IRequestHandler<ConvertQuotationToSalesOrderCommand, ConvertQuotationToSalesOrderResponse>
{
    public async Task<ConvertQuotationToSalesOrderResponse> Handle(ConvertQuotationToSalesOrderCommand request, CancellationToken cancellationToken)
    {
        var quotation = db.QuotationsWithLines.SingleOrDefault(x => x.Id == request.Id)
            ?? throw new NotFoundException("Quotation not found.");
        var existingOrder = db.SalesOrdersWithLines.SingleOrDefault(x => x.SourceQuotationId == quotation.Id);
        if (existingOrder is not null)
            return new(BillingResponseMapper.Map(existingOrder));
        if (quotation.Status != QuotationStatus.Accepted)
            throw new ConflictException("Only an accepted quotation can be converted.");

        var warehouse = db.Warehouses.SingleOrDefault(x => x.Id == request.WarehouseId)
            ?? throw new NotFoundException("Warehouse not found.");
        if (!warehouse.IsActive) throw new ConflictException("Inactive warehouse cannot be used.");
        var userId = actor.UserId ?? throw new AuthenticationFailureException();
        var lines = quotation.Lines.Select(x => new SalesOrderLine(
            x.ProductId, x.Quantity, x.UnitPrice, x.DiscountAmount, x.TaxCodeId, x.TaxRate, x.Id));
        var order = new SalesOrder(quotation.CustomerId, request.WarehouseId, quotation.Id,
            DateOnly.FromDateTime(clock.UtcNow.UtcDateTime), request.DeliveryDate, request.DeliveryAddress,
            lines, clock.UtcNow);
        db.Add(order);
        SecurityAuditWriter.Add(db, userId, "QuotationConvertedToSalesOrder", "SalesOrder", order.Id.ToString(),
            null, quotation.Id.ToString());
        await db.SaveChangesAsync(cancellationToken);
        return new ConvertQuotationToSalesOrderResponse(BillingResponseMapper.Map(order));
    }
}
