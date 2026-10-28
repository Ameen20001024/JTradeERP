using FluentValidation;
namespace JTradeERP.Application.Features.Inventory.Adjustments.ReverseStockAdjustment;
public sealed class ReverseStockAdjustmentValidator:AbstractValidator<ReverseStockAdjustmentCommand>{public ReverseStockAdjustmentValidator()=>RuleFor(x=>x.Id).NotEmpty();}
