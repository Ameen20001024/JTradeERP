using MediatR;
namespace JTradeERP.Application.Features.Billing.SalesOrders.GetSalesOrderById;
public sealed record GetSalesOrderByIdQuery(Guid Id):IRequest<GetSalesOrderByIdResponse>;
