using FluentValidation;
namespace JTradeERP.Application.Features.Inventory.StockLedger.GetStockLedger;
public sealed class GetStockLedgerValidator:AbstractValidator<GetStockLedgerQuery>
{ public GetStockLedgerValidator(){RuleFor(x=>x.Page).GreaterThan(0);RuleFor(x=>x.PageSize).InclusiveBetween(1,500);RuleFor(x=>x.To).GreaterThanOrEqualTo(x=>x.From).When(x=>x.From.HasValue&&x.To.HasValue);RuleFor(x=>x.SourceDocumentType).MaximumLength(64);} }
