using System.Reflection;
using System.Text.Json;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Billing;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Common.Behaviors;

public sealed class BillingAuditBehavior<TRequest, TResponse>(
    IApplicationDbContext db,
    ICurrentUserService currentUser) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestType = request.GetType();
        if (!requestType.Namespace?.Contains("Features.Billing", StringComparison.Ordinal) == true ||
            !requestType.Name.EndsWith("Command", StringComparison.Ordinal))
            return await next();

        var command = requestType.Name[..^"Command".Length];
        var entity = EntityFor(command);
        var requestId = ReadGuidProperty(request, "Id");
        var previous = requestId.HasValue ? ReadSnapshot(entity, requestId.Value) : null;
        var response = await next();
        var entityId = ReadGuidProperty(response!, "Id") ?? ReadGuidProperty(request, "Id");
        if (entityId is null) return response;

        var action = ActionFor(command, entity);
        var id = entityId.Value.ToString();
        if (!db.SecurityAuditEntries.Any(x => x.Entity == entity && x.EntityId == id && x.Action == action))
        {
            var current = ReadSnapshot(entity, entityId.Value);
            SecurityAuditWriter.Add(db, currentUser.UserId, action, entity, id, previous, current);
            await db.SaveChangesAsync(cancellationToken);
        }
        return response;
    }

    private string? ReadSnapshot(string entity, Guid id)
    {
        object? value = entity switch
        {
            "Quotation" => db.QuotationsWithLines.Where(x => x.Id == id).Select(x => new
            {
                x.Id, x.CustomerId, x.QuotationDate, x.ValidityDate, x.Status, x.SubTotal, x.DiscountAmount,
                x.TaxAmount, x.GrandTotal,
                Lines = x.Lines.Select(line => new { line.ProductId, line.Quantity, line.UnitPrice, line.DiscountAmount, line.TaxCodeId, line.TaxRate, line.LineTotal })
            }).SingleOrDefault(),
            "SalesOrder" => db.SalesOrdersWithLines.Where(x => x.Id == id).Select(x => new
            {
                x.Id, x.CustomerId, x.WarehouseId, x.OrderDate, x.DeliveryDate, x.Status, x.SubTotal,
                x.DiscountAmount, x.TaxAmount, x.GrandTotal,
                Lines = x.Lines.Select(line => new { line.ProductId, line.OrderedQuantity, line.ReservedQuantity, line.FulfilledQuantity, line.UnitPrice, line.TaxCodeId, line.LineTotal })
            }).SingleOrDefault(),
            "Invoice" => db.InvoicesWithLines.Where(x => x.Id == id).Select(x => new
            {
                x.Id, x.InvoiceNumber, x.CustomerId, x.WarehouseId, x.InvoiceDate, x.Status, x.SubTotal,
                x.DiscountAmount, x.TaxAmount, x.GrandTotal, x.PaidAmount, x.OutstandingAmount,
                Lines = x.Lines.Select(line => new { line.ProductId, line.Quantity, line.UnitPrice, line.DiscountAmount, line.TaxCodeId, line.TaxRateSnapshot, line.TaxAmountSnapshot, line.LineTotal })
            }).SingleOrDefault(),
            "CreditNote" => db.CreditNotesWithLines.Where(x => x.Id == id).Select(x => new
            {
                x.Id, x.InvoiceId, x.CreditNoteDate, x.Status, x.SubTotal, x.TaxAmount, x.GrandTotal,
                Lines = x.Lines.Select(line => new { line.InvoiceLineId, line.ProductId, line.Quantity, line.UnitPrice, line.TaxRateSnapshot, line.TaxAmount, line.LineTotal })
            }).SingleOrDefault(),
            "Payment" => db.PaymentsWithAllocations.Where(x => x.Id == id).Select(x => new
            {
                x.Id, x.PaymentDate, x.PaymentMethodId, x.Amount, x.ReferenceNumber, x.Status,
                Allocations = x.Allocations.Select(allocation => new { allocation.InvoiceId, allocation.Amount })
            }).SingleOrDefault(),
            "PaymentMethod" => db.PaymentMethods.Where(x => x.Id == id).Select(x => new { x.Id, x.Code, x.Name, x.IsActive }).SingleOrDefault(),
            "NumberSeries" => db.NumberSeries.Where(x => x.Id == id).Select(x => new { x.Id, x.DocumentType, x.Prefix, x.NextNumber, x.Padding }).SingleOrDefault(),
            "BillingConfiguration" => db.BillingConfigurations.Where(x => x.Id == id).Select(x => new { x.Id, x.CompanyTrn }).SingleOrDefault(),
            _ => null
        };
        return value is null ? null : JsonSerializer.Serialize(value);
    }

    private static string EntityFor(string command)
    {
        if (command.Contains("SalesOrder", StringComparison.Ordinal)) return "SalesOrder";
        if (command.Contains("CreditNote", StringComparison.Ordinal)) return "CreditNote";
        if (command.Contains("Invoice", StringComparison.Ordinal)) return "Invoice";
        if (command.Contains("Quotation", StringComparison.Ordinal)) return "Quotation";
        if (command.Contains("PaymentMethod", StringComparison.Ordinal)) return "PaymentMethod";
        if (command.Contains("Payment", StringComparison.Ordinal)) return "Payment";
        if (command.Contains("NumberSeries", StringComparison.Ordinal)) return "NumberSeries";
        return "BillingConfiguration";
    }

    private static string ActionFor(string command, string entity) => command switch
    {
        "ConvertQuotationToSalesOrder" => "QuotationConvertedToSalesOrder",
        "RecordPayment" => "PaymentRecorded",
        "ReversePayment" => "PaymentReversed",
        "UpdateBillingConfiguration" => "BillingConfigurationUpdated",
        _ => entity + command switch
        {
            var op when op.StartsWith("Create", StringComparison.Ordinal) => "Created",
            var op when op.StartsWith("Update", StringComparison.Ordinal) => "Updated",
            var op when op.StartsWith("Post", StringComparison.Ordinal) => "Posted",
            var op when op.StartsWith("Cancel", StringComparison.Ordinal) => "Cancelled",
            var op when op.StartsWith("Send", StringComparison.Ordinal) => "Sent",
            var op when op.StartsWith("Accept", StringComparison.Ordinal) => "Accepted",
            var op when op.StartsWith("Reject", StringComparison.Ordinal) => "Rejected",
            var op when op.StartsWith("Expire", StringComparison.Ordinal) => "Expired",
            var op when op.StartsWith("Copy", StringComparison.Ordinal) => "Copied",
            var op when op.StartsWith("Activate", StringComparison.Ordinal) => "Activated",
            var op when op.StartsWith("Deactivate", StringComparison.Ordinal) => "Deactivated",
            var op when op.StartsWith("Delete", StringComparison.Ordinal) => "Deleted",
            _ => "Changed"
        }
    };

    private static Guid? ReadGuidProperty(object? source, string propertyName)
    {
        if (source is null) return null;
        var property = source.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        if (property?.PropertyType == typeof(Guid) && property.GetValue(source) is Guid id) return id;
        foreach (var nested in source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                     .Where(x => x.CanRead && x.PropertyType != typeof(string) && !typeof(System.Collections.IEnumerable).IsAssignableFrom(x.PropertyType)))
        {
            if (ReadGuidProperty(nested.GetValue(source), propertyName) is { } nestedId) return nestedId;
        }
        return null;
    }
}
