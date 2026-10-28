using FluentValidation;
namespace JTradeERP.Application.Features.MasterData.Categories.CreateCategory;
public sealed class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand> { public CreateCategoryValidator() => RuleFor(x => x.Name).NotEmpty().MaximumLength(200); }
