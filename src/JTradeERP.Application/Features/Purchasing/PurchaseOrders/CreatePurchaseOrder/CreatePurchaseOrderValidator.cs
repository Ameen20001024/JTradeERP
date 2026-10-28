using FluentValidation;
using JTradeERP.Application.Features.Purchasing.Common;

namespace JTradeERP.Application.Features.Purchasing.PurchaseOrders.CreatePurchaseOrder;

public sealed class CreatePurchaseOrderValidator : AbstractValidator<CreatePurchaseOrderCommand>
{
    public CreatePurchaseOrderValidator()
    {
        RuleFor(x => x.SupplierId).NotEmpty();
        RuleFor(x => x.WarehouseId).NotEmpty();
        RuleFor(x => x.OrderDate).NotEmpty();
        RuleFor(x => x.ExpectedDate).Must((x, date) => !date.HasValue || date.Value >= x.OrderDate)
            .WithMessage("Expected date cannot precede order date.");
        RuleFor(x => x.Lines).NotNull().NotEmpty();
        RuleForEach(x => x.Lines).ChildRules(line =>
        {
            line.RuleFor(x => x.ProductId).NotEmpty();
            line.RuleFor(x => x.OrderedQuantity).GreaterThan(0).Must(PurchasingPrecision.Quantity);
            line.RuleFor(x => x.UnitPurchaseCost).GreaterThanOrEqualTo(0).Must(PurchasingPrecision.UnitCost);
            line.RuleFor(x => x.TaxCodeId).NotEmpty();
            line.RuleFor(x => x.DiscountAmount).GreaterThanOrEqualTo(0).Must(PurchasingPrecision.Money);
        });
    }
}
