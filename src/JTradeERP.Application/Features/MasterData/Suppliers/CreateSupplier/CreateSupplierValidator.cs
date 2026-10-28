using FluentValidation;
namespace JTradeERP.Application.Features.MasterData.Suppliers.CreateSupplier;
public sealed class CreateSupplierValidator:AbstractValidator<CreateSupplierCommand>{public CreateSupplierValidator(){RuleFor(x=>x.SupplierCode).NotEmpty().MaximumLength(50);RuleFor(x=>x.Name).NotEmpty().MaximumLength(200);}}
