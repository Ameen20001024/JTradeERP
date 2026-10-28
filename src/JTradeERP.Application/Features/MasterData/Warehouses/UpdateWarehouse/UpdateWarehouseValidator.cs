using FluentValidation;
namespace JTradeERP.Application.Features.MasterData.Warehouses.UpdateWarehouse;
public sealed class UpdateWarehouseValidator:AbstractValidator<UpdateWarehouseCommand>{public UpdateWarehouseValidator(){RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.WarehouseCode).NotEmpty().MaximumLength(50);RuleFor(x=>x.Name).NotEmpty().MaximumLength(200);}}
