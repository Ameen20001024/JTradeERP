using MediatR;
namespace JTradeERP.Application.Features.Inventory.ReorderAlerts.GetReorderAlerts;
public sealed record GetReorderAlertsQuery(Guid? WarehouseId=null):IRequest<GetReorderAlertsResponse>;
