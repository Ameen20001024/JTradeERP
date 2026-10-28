using JTradeERP.Domain.Purchasing;
using MediatR;

namespace JTradeERP.Application.Features.Purchasing.PurchaseOrders.GetPurchaseOrders;

public sealed record GetPurchaseOrdersQuery(Guid? SupplierId = null, Guid? WarehouseId = null,
    PurchaseOrderStatus? Status = null, int PageNumber = 1, int PageSize = 50)
    : IRequest<GetPurchaseOrdersResponse>;
