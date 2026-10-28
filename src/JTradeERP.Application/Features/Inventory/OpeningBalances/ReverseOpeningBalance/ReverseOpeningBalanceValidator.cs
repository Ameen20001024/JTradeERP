using FluentValidation;
namespace JTradeERP.Application.Features.Inventory.OpeningBalances.ReverseOpeningBalance;
public sealed class ReverseOpeningBalanceValidator : AbstractValidator<ReverseOpeningBalanceCommand>
{ public ReverseOpeningBalanceValidator() => RuleFor(x => x.Id).NotEmpty(); }
