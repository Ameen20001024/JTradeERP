using FluentValidation;

namespace JTradeERP.Application.Features.Purchasing.PurchaseOrders.GetPurchaseOrders;

public sealed class GetPurchaseOrdersValidator : AbstractValidator<GetPurchaseOrdersQuery>
{
    public GetPurchaseOrdersValidator()
    {
        RuleFor(x => x.SupplierId).Must(x => !x.HasValue || x.Value != Guid.Empty);
        RuleFor(x => x.WarehouseId).Must(x => !x.HasValue || x.Value != Guid.Empty);
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
    }
}
