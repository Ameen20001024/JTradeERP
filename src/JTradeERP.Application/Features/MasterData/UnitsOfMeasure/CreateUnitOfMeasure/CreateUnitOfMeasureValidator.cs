using FluentValidation;
namespace JTradeERP.Application.Features.MasterData.UnitsOfMeasure.CreateUnitOfMeasure;
public sealed class CreateUnitOfMeasureValidator : AbstractValidator<CreateUnitOfMeasureCommand> { public CreateUnitOfMeasureValidator(){RuleFor(x=>x.Code).NotEmpty().MaximumLength(50);RuleFor(x=>x.Name).NotEmpty().MaximumLength(200);} }
