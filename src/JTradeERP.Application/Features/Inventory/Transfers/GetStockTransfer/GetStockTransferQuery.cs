using MediatR;
namespace JTradeERP.Application.Features.Inventory.Transfers.GetStockTransfer;
public sealed record GetStockTransferQuery(Guid Id) : IRequest<GetStockTransferResponse>;
