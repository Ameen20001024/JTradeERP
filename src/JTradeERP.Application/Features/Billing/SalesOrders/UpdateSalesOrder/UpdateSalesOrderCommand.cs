using MediatR;
using JTradeERP.Application.Features.Billing.Common;

namespace JTradeERP.Application.Features.Billing.SalesOrders.UpdateSalesOrder;

public sealed record UpdateSalesOrderCommand(
    Guid Id,
    Guid CustomerId,
    Guid WarehouseId,
    DateOnly OrderDate,
    DateOnly? DeliveryDate,
    string? DeliveryAddress,
    IReadOnlyCollection<SalesOrderLineInput> Lines):IRequest<UpdateSalesOrderResponse>;
