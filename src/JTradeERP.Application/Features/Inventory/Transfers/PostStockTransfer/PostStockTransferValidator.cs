using FluentValidation;
namespace JTradeERP.Application.Features.Inventory.Transfers.PostStockTransfer;
public sealed class PostStockTransferValidator : AbstractValidator<PostStockTransferCommand>
{ public PostStockTransferValidator() => RuleFor(x => x.Id).NotEmpty(); }
