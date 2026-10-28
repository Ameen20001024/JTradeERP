using FluentValidation;
namespace JTradeERP.Application.Features.Inventory.ReorderAlerts.GetReorderAlerts;
public sealed class GetReorderAlertsValidator:AbstractValidator<GetReorderAlertsQuery>
{ public GetReorderAlertsValidator(){RuleFor(x=>x.WarehouseId).NotEqual(Guid.Empty).When(x=>x.WarehouseId.HasValue);} }
