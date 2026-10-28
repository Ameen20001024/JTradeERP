using FluentValidation;
namespace JTradeERP.Application.Features.Inventory.Adjustments.GetStockAdjustment;
public sealed class GetStockAdjustmentValidator:AbstractValidator<GetStockAdjustmentQuery>{public GetStockAdjustmentValidator()=>RuleFor(x=>x.Id).NotEmpty();}
