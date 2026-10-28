using FluentValidation;using JTradeERP.Application.Common.Inventory;
namespace JTradeERP.Application.Features.Inventory.Adjustments.PostStockAdjustment;
public sealed class PostStockAdjustmentValidator:AbstractValidator<PostStockAdjustmentCommand>{public PostStockAdjustmentValidator(){RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.UnitCost).Must(x=>!x.HasValue||InventoryPrecision.IsUnitCost(x.Value));}}
