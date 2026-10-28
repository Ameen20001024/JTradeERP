using JTradeERP.Application.Features.Purchasing.Common;
using MediatR;

namespace JTradeERP.Application.Features.Purchasing.PurchaseOrders.UpdatePurchaseOrder;

public sealed record UpdatePurchaseOrderCommand(Guid Id, Guid SupplierId, Guid WarehouseId,
    DateOnly OrderDate, DateOnly? ExpectedDate, IReadOnlyCollection<PurchaseOrderLineInput> Lines)
    : IRequest<UpdatePurchaseOrderResponse>;
