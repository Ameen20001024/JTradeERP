using FluentValidation;

namespace JTradeERP.Application.Features.Purchasing.PurchaseOrders.GetPurchaseOrderById;

public sealed class GetPurchaseOrderByIdValidator : AbstractValidator<GetPurchaseOrderByIdQuery>
{
    public GetPurchaseOrderByIdValidator() => RuleFor(x => x.Id).NotEmpty();
}
