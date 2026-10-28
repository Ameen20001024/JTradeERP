using MediatR;
using JTradeERP.Application.Features.Inventory.Transfers;
namespace JTradeERP.Application.Features.Inventory.Transfers.CreateStockTransfer;
public sealed record CreateStockTransferCommand(Guid SourceWarehouseId, Guid DestinationWarehouseId,
    IReadOnlyCollection<StockTransferLineRequest> Lines) : IRequest<CreateStockTransferResponse>;
