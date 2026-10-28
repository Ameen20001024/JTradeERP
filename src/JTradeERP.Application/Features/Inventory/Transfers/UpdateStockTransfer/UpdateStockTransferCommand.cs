using MediatR;
using JTradeERP.Application.Features.Inventory.Transfers;
namespace JTradeERP.Application.Features.Inventory.Transfers.UpdateStockTransfer;
public sealed record UpdateStockTransferCommand(Guid Id, Guid SourceWarehouseId, Guid DestinationWarehouseId,
    IReadOnlyCollection<StockTransferLineRequest> Lines) : IRequest<UpdateStockTransferResponse>;
