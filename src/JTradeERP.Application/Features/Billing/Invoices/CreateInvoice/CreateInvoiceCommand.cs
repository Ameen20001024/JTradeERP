using MediatR;
using JTradeERP.Application.Features.Billing.Common;

namespace JTradeERP.Application.Features.Billing.Invoices.CreateInvoice;

public sealed record CreateInvoiceCommand(
    Guid CustomerId,
    Guid WarehouseId,
    Guid? SourceSalesOrderId,
    DateOnly InvoiceDate,
    IReadOnlyCollection<InvoiceLineInput> Lines):IRequest<CreateInvoiceResponse>;
