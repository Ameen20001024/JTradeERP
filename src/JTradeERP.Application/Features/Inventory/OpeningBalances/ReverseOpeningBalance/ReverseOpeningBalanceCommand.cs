using MediatR;
namespace JTradeERP.Application.Features.Inventory.OpeningBalances.ReverseOpeningBalance;
public sealed record ReverseOpeningBalanceCommand(Guid Id) : IRequest<ReverseOpeningBalanceResponse>;
