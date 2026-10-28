using FluentValidation;using JTradeERP.Application.Common.Inventory;
namespace JTradeERP.Application.Features.Inventory.Adjustments.UpdateStockAdjustment;
public sealed class UpdateStockAdjustmentValidator:AbstractValidator<UpdateStockAdjustmentCommand>
{ public UpdateStockAdjustmentValidator(){RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.ProductId).NotEmpty();RuleFor(x=>x.WarehouseId).NotEmpty();RuleFor(x=>x.QuantityDelta).NotEqual(0).Must(InventoryPrecision.IsQuantity);RuleFor(x=>x.ReasonCode).NotEmpty().MaximumLength(100);} }
