using FluentValidation;using JTradeERP.Application.Common.Inventory;
namespace JTradeERP.Application.Features.Inventory.Adjustments.CreateStockAdjustment;
public sealed class CreateStockAdjustmentValidator:AbstractValidator<CreateStockAdjustmentCommand>
{ public CreateStockAdjustmentValidator(){RuleFor(x=>x.ProductId).NotEmpty();RuleFor(x=>x.WarehouseId).NotEmpty();RuleFor(x=>x.QuantityDelta).NotEqual(0).Must(InventoryPrecision.IsQuantity);RuleFor(x=>x.ReasonCode).NotEmpty().MaximumLength(100);} }
