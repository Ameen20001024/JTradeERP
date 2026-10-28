using FluentValidation;
namespace JTradeERP.Application.Features.Inventory.StockBalances.GetStockBalances;
public sealed class GetStockBalancesValidator : AbstractValidator<GetStockBalancesQuery>
{ public GetStockBalancesValidator(){RuleFor(x=>x.Page).GreaterThan(0);RuleFor(x=>x.PageSize).InclusiveBetween(1,200);} }
