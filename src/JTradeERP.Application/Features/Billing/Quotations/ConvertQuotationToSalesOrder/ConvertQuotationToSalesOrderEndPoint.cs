using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;
namespace JTradeERP.Application.Features.Billing.Quotations.ConvertQuotationToSalesOrder;
public static class ConvertQuotationToSalesOrderEndPoint
{
    public static void Map(IEndpointRouteBuilder e)=>
        e.MapPost(
            "/quotations/{id:guid}/convert-to-sales-order",
            async(Guid id,
                Guid warehouseId,
                DateOnly? deliveryDate,
                string? deliveryAddress,
                ISender s,
                CancellationToken ct) =>
            Results.Created(
                "/api/billing/sales-orders",
                await s.Send(new ConvertQuotationToSalesOrderCommand(
                    id,
                    warehouseId,
                    deliveryDate,
                    deliveryAddress),
                    ct))
        ).RequirePermission("Billing.Quotation",Capability.CreateEdit).
        RequirePermission("Billing.SalesOrder",Capability.CreateEdit);
}
