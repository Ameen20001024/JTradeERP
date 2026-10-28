using MediatR;

namespace JTradeERP.Application.Features.Purchasing.PurchaseOrders.GetPurchaseOrderById;

public sealed record GetPurchaseOrderByIdQuery(Guid Id) : IRequest<GetPurchaseOrderByIdResponse>;
