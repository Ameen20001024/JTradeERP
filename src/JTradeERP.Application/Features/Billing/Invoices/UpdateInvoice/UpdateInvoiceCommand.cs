using MediatR;
using JTradeERP.Application.Features.Billing.Common;

namespace JTradeERP.Application.Features.Billing.Invoices.UpdateInvoice;

public sealed record UpdateInvoiceCommand
    (Guid Id,
    Guid CustomerId,
    Guid WarehouseId,
    DateOnly InvoiceDate,
    IReadOnlyCollection<InvoiceLineInput> Lines):IRequest<UpdateInvoiceResponse>;
