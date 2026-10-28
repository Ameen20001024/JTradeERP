using FluentValidation;
namespace JTradeERP.Application.Features.MasterData.Warehouses.CreateWarehouse;
public sealed class CreateWarehouseValidator:AbstractValidator<CreateWarehouseCommand>{public CreateWarehouseValidator(){RuleFor(x=>x.WarehouseCode).NotEmpty().MaximumLength(50);RuleFor(x=>x.Name).NotEmpty().MaximumLength(200);}}
