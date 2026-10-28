using MediatR;using JTradeERP.Application.Features.Billing.Common;
namespace JTradeERP.Application.Features.Billing.SalesOrders.CreateSalesOrder;
public sealed record CreateSalesOrderCommand(
    Guid CustomerId,
    Guid WarehouseId,
    Guid? SourceQuotationId,
    DateOnly OrderDate,
    DateOnly? DeliveryDate,
    string? DeliveryAddress,
    IReadOnlyCollection<SalesOrderLineInput> Lines):IRequest<CreateSalesOrderResponse>;
