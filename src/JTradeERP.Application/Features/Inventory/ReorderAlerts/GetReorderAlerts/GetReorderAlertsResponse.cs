using JTradeERP.Application.Common.Inventory;
namespace JTradeERP.Application.Features.Inventory.ReorderAlerts.GetReorderAlerts;
public sealed record GetReorderAlertsResponse(IReadOnlyCollection<ReorderAlertItemResponse> Items,int TotalCount);
