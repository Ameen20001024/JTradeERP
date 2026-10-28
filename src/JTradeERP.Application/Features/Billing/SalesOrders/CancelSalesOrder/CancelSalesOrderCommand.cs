using MediatR;
namespace JTradeERP.Application.Features.Billing.SalesOrders.CancelSalesOrder;

public sealed record CancelSalesOrderCommand(Guid Id):IRequest<CancelSalesOrderResponse>;
