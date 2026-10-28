using MediatR;
namespace JTradeERP.Application.Features.Billing.Quotations.ConvertQuotationToSalesOrder;

public sealed record ConvertQuotationToSalesOrderCommand(Guid Id,
    Guid WarehouseId,
    DateOnly? DeliveryDate,
    string? DeliveryAddress):IRequest<ConvertQuotationToSalesOrderResponse>;
