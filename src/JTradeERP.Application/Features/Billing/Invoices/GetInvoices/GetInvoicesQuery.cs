using MediatR;
using JTradeERP.Domain.Billing;

namespace JTradeERP.Application.Features.Billing.Invoices.GetInvoices;

public sealed record GetInvoicesQuery(
    Guid? CustomerId=null,
    InvoiceStatus? Status=null,
    DateOnly? From=null,
    DateOnly? To=null,
    int Page=1,
    int PageSize=50):IRequest<GetInvoicesResponse>;
