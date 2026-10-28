using MediatR;
using JTradeERP.Application.Features.Billing.Common;

namespace JTradeERP.Application.Features.Billing.Invoices.GetInvoiceById;

public sealed record GetInvoiceByIdQuery(Guid Id):IRequest<GetInvoiceByIdResponse>;
