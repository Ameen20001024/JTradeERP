using MediatR;

namespace JTradeERP.Application.Features.Billing.Invoices.PostInvoice;

public sealed record PostInvoiceCommand(Guid Id):IRequest<PostInvoiceResponse>;
