using FluentValidation;
namespace JTradeERP.Application.Features.Inventory.StockBalances.GetStockBalance;
public sealed class GetStockBalanceValidator : AbstractValidator<GetStockBalanceQuery>
{ public GetStockBalanceValidator(){RuleFor(x=>x.ProductId).NotEmpty();RuleFor(x=>x.WarehouseId).NotEmpty();} }
