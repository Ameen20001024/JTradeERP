using MediatR;
using JTradeERP.Domain.Billing;

namespace JTradeERP.Application.Features.Billing.Quotations.GetQuotations;

public sealed record GetQuotationsQuery(
    Guid? CustomerId=null, 
    QuotationStatus? Status=null,
    int Page=1,
    int PageSize=50):IRequest<GetQuotationsResponse>;
