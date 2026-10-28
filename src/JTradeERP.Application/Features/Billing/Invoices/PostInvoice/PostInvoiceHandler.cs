using System.Diagnostics;
using JTradeERP.Application.Common.Billing;
using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Inventory;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Application.Features.Billing.Common;
using JTradeERP.Domain.Billing;
using JTradeERP.Domain.Inventory;
using MediatR;

namespace JTradeERP.Application.Features.Billing.Invoices.PostInvoice;

public sealed class PostInvoiceHandler(
    IApplicationDbContext db,
    IStockPostingService stock,
    INumberSeriesAllocator numberSeries,
    ICurrentUserService actor,
    IClock clock) : IRequestHandler<PostInvoiceCommand, PostInvoiceResponse>
{
    public async Task<PostInvoiceResponse> Handle(PostInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = db.InvoicesWithLines.SingleOrDefault(x => x.Id == request.Id)
            ?? throw new NotFoundException("Invoice not found.");

        if (invoice.Status is InvoiceStatus.Posted or InvoiceStatus.PartiallyPaid or InvoiceStatus.Paid)
            return new(BillingResponseMapper.Map(invoice));

        if (invoice.Status != InvoiceStatus.Draft)
            throw new ConflictException("Only a draft invoice can be posted.");

        var userId = actor.UserId ?? throw new AuthenticationFailureException();

        await using var transaction = await db.BeginTransactionAsync(cancellationToken);

        var customer = db.Customers.SingleOrDefault(x => x.Id == invoice.CustomerId)
            ?? throw new NotFoundException("Customer not found.");

        var taxRates = new Dictionary<Guid, decimal>();

        foreach (var taxCodeId in invoice.Lines.Select(x => x.TaxCodeId).Distinct())
        {
            var taxCode = db.TaxCodes.SingleOrDefault(x => x.Id == taxCodeId)
                ?? throw new NotFoundException("Tax code not found.");

            if (!taxCode.IsActive) throw new ConflictException("Inactive tax codes cannot be posted.");

            taxRates.Add(taxCodeId, taxCode.Rate);
        }

        invoice.SnapshotTaxRates(taxRates);

        var invoiceNumber = await numberSeries.AllocateAsync("Invoice", cancellationToken);

        var companyTrn = db.BillingConfigurations.SingleOrDefault(x => x.Id == BillingConfiguration.SingletonId)?.CompanyTrn;

        invoice.Post(invoiceNumber, companyTrn, customer.TRN, clock.UtcNow, userId);

        var salesOrder = invoice.SourceSalesOrderId is { } salesOrderId
            ? db.SalesOrdersWithLines.SingleOrDefault(x => x.Id == salesOrderId)
                ?? throw new NotFoundException("Sales order not found.")
            : null;

        var correlationId = Activity.Current?.Id ?? Guid.NewGuid().ToString();

        foreach (var line in invoice.Lines)
        {
            stock.EnsureActiveReferences(line.ProductId, invoice.WarehouseId);
            if (salesOrder is null)
            {
                stock.Issue(
                    line.ProductId,
                    invoice.WarehouseId,
                    line.Quantity,
                    "Invoice",
                    invoice.Id,
                    line.Id,
                    StockMovementType.Issue,
                    userId,
                    correlationId);

                continue;
            }

            var salesOrderLineId = line.SourceSalesOrderLineId
                ?? throw new ConflictException("Sales-order invoice line is missing its source line.");

            var orderLine = salesOrder.Lines.SingleOrDefault(x => x.Id == salesOrderLineId)
                ?? throw new ConflictException("Invoice line does not belong to its sales order.");

            if (orderLine.ProductId != line.ProductId || line.Quantity > orderLine.ReservedQuantity)
                throw new ConflictException("Invoice quantity exceeds the matching reservation.");

            stock.ConvertReservationToIssue(
                line.ProductId,
                invoice.WarehouseId,
                line.Quantity,
                "Invoice",
                invoice.Id,
                line.Id,
                userId,
                correlationId);

            salesOrder.Fulfill(orderLine.Id, line.Quantity, clock.UtcNow);
        }

        SecurityAuditWriter.Add(db,
            userId,
            "InvoicePosted",
            "Invoice",
            invoice.Id.ToString(),
            "Draft",
            $"{invoiceNumber}|{invoice.GrandTotal}");


        await db.SaveChangesAsync(cancellationToken);
        
        await transaction.CommitAsync(cancellationToken);
        
        return new PostInvoiceResponse(BillingResponseMapper.Map(invoice));
    }
}
