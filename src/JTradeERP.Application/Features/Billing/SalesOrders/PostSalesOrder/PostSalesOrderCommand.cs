using MediatR;

namespace JTradeERP.Application.Features.Billing.SalesOrders.PostSalesOrder;

public sealed record PostSalesOrderCommand(Guid Id):IRequest<PostSalesOrderResponse>;
