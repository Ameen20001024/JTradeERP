using JTradeERP.Application.Features.Purchasing.Common;
using MediatR;

namespace JTradeERP.Application.Features.Purchasing.PurchaseOrders.CreatePurchaseOrder;

public sealed record CreatePurchaseOrderCommand(Guid SupplierId, Guid WarehouseId, DateOnly OrderDate,
    DateOnly? ExpectedDate, IReadOnlyCollection<PurchaseOrderLineInput> Lines)
    : IRequest<CreatePurchaseOrderResponse>;
