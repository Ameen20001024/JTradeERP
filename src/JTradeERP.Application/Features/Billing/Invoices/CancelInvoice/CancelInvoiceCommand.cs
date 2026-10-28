using MediatR;

namespace JTradeERP.Application.Features.Billing.Invoices.CancelInvoice;

public sealed record CancelInvoiceCommand(Guid Id):IRequest<CancelInvoiceResponse>;
