using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using MediatR;
namespace JTradeERP.Application.Features.Inventory.Transfers.GetStockTransfer;
public sealed class GetStockTransferHandler(IApplicationDbContext db)
    : IRequestHandler<GetStockTransferQuery, GetStockTransferResponse>
{
    public Task<GetStockTransferResponse> Handle(GetStockTransferQuery request, CancellationToken cancellationToken)
    {
        var transfer = db.StockTransfers.SingleOrDefault(x => x.Id == request.Id)
            ?? throw new NotFoundException("Stock transfer was not found.");
        var lines = db.StockTransferLines.Where(x => x.StockTransferId == transfer.Id).ToArray();
        return Task.FromResult(new GetStockTransferResponse(StockTransferResponseMapper.Map(db, transfer, lines)));
    }
}
