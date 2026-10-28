using FluentValidation;
namespace JTradeERP.Application.Features.MasterData.Suppliers.UpdateSupplier;
public sealed class UpdateSupplierValidator:AbstractValidator<UpdateSupplierCommand>{public UpdateSupplierValidator(){RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.SupplierCode).NotEmpty().MaximumLength(50);RuleFor(x=>x.Name).NotEmpty().MaximumLength(200);}}
