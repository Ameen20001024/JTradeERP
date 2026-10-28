using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Domain.Billing;

namespace JTradeERP.Application.Features.Billing.Common;

internal static class BillingReferences
{
    public static void Customer(IApplicationDbContext db, Guid id) =>
        _ = db.Customers.SingleOrDefault(x => x.Id == id) ?? throw new NotFoundException("Customer not found.");

    public static void ActiveCustomer(IApplicationDbContext db, Guid id) 
    {
        var customer = db.Customers.SingleOrDefault(x => x.Id == id) ?? throw new NotFoundException("Customer not found.");
    }
    public static QuotationLine QuotationLine(IApplicationDbContext db, BillingLineInput x)
    {
        var p=db.Products.SingleOrDefault(y=>y.Id==x.ProductId)??throw new NotFoundException("Product not found.");
        var t=db.TaxCodes.SingleOrDefault(y=>y.Id==x.TaxCodeId)??throw new NotFoundException("Tax code not found."); 
        if(!p.IsActive||!t.IsActive)throw new ConflictException("Inactive products or tax codes cannot be used."); 
        return new(
            x.ProductId,
            x.Quantity,
            x.UnitPrice,
            x.DiscountAmount,
            x.TaxCodeId,
            t.Rate);
    }

    public static InvoiceLine InvoiceLine(IApplicationDbContext db, InvoiceLineInput x) 
    {
        var p = db.Products.SingleOrDefault(y=>y.Id==x.ProductId)??throw new NotFoundException("Product not found.");
        var t = db.TaxCodes.SingleOrDefault(y=>y.Id==x.TaxCodeId)??throw new NotFoundException("Tax code not found.");

        if(!p.IsActive||!t.IsActive)
            throw new ConflictException("Inactive products or tax codes cannot be invoiced.");
        
        return new(
            x.ProductId,
            x.SourceSalesOrderLineId,
            x.Quantity,
            x.UnitPrice,
            x.DiscountAmount,
            x.TaxCodeId,
            t.Rate);
    }

    public static T Required<T>(T? value, string message) where T : class => value ?? throw new NotFoundException(message);
}
