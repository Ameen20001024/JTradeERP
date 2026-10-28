using FluentValidation;using JTradeERP.Domain.Inventory;
namespace JTradeERP.Application.Features.Inventory.Adjustments.GetStockAdjustments;
public sealed class GetStockAdjustmentsValidator:AbstractValidator<GetStockAdjustmentsQuery>{public GetStockAdjustmentsValidator(){RuleFor(x=>x.Page).GreaterThan(0);RuleFor(x=>x.PageSize).InclusiveBetween(1,200);RuleFor(x=>x.Status).Must(v=>string.IsNullOrWhiteSpace(v)||Enum.TryParse<InventoryDocumentStatus>(v,true,out _)).WithMessage("Status is invalid.");}}
