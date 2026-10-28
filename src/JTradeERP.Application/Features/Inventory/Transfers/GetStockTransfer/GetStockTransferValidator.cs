using FluentValidation;
namespace JTradeERP.Application.Features.Inventory.Transfers.GetStockTransfer;
public sealed class GetStockTransferValidator : AbstractValidator<GetStockTransferQuery>
{ public GetStockTransferValidator() => RuleFor(x => x.Id).NotEmpty(); }
