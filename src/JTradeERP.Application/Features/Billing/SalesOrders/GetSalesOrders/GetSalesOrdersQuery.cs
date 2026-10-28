using MediatR;
using JTradeERP.Domain.Billing;

namespace JTradeERP.Application.Features.Billing.SalesOrders.GetSalesOrders;

public sealed record GetSalesOrdersQuery(
    Guid? CustomerId=null,
    SalesOrderStatus? Status=null,
    int Page=1,
    int PageSize=50):IRequest<GetSalesOrdersResponse>;
