using FluentValidation;
namespace JTradeERP.Application.Features.MasterData.UnitsOfMeasure.UpdateUnitOfMeasure;
public sealed class UpdateUnitOfMeasureValidator : AbstractValidator<UpdateUnitOfMeasureCommand> { public UpdateUnitOfMeasureValidator(){RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Code).NotEmpty().MaximumLength(50);RuleFor(x=>x.Name).NotEmpty().MaximumLength(200);} }
