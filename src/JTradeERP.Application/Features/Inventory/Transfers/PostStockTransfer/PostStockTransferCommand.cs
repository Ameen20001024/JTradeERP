using MediatR;
namespace JTradeERP.Application.Features.Inventory.Transfers.PostStockTransfer;
public sealed record PostStockTransferCommand(Guid Id) : IRequest<PostStockTransferResponse>;
