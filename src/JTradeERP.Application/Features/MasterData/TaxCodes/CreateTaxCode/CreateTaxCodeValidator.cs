using FluentValidation;
namespace JTradeERP.Application.Features.MasterData.TaxCodes.CreateTaxCode;
public sealed class CreateTaxCodeValidator : AbstractValidator<CreateTaxCodeCommand> { public CreateTaxCodeValidator() { RuleFor(x => x.Code).NotEmpty().MaximumLength(50); RuleFor(x => x.Name).NotEmpty().MaximumLength(200); RuleFor(x => x.TaxType).NotEmpty().MaximumLength(50); RuleFor(x => x.Rate).InclusiveBetween(0m, 100m); } }
