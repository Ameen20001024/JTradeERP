using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Inventory;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Domain.Inventory;
using MediatR;

namespace JTradeERP.Application.Features.Inventory.Adjustments.GetStockAdjustment;

public sealed class GetStockAdjustmentHandler(IApplicationDbContext db)
    : IRequestHandler<GetStockAdjustmentQuery, GetStockAdjustmentResponse>
{
    public Task<GetStockAdjustmentResponse> Handle(GetStockAdjustmentQuery request, CancellationToken cancellationToken)
    {
        var adjustment = db.StockAdjustments.SingleOrDefault(x => x.Id == request.Id)
            ?? throw new NotFoundException("Stock adjustment was not found.");
        var ledger = db.StockLedgerEntries.SingleOrDefault(x =>
            x.SourceDocumentType == "StockAdjustment" && x.SourceDocumentId == adjustment.Id
            && x.MovementType == StockMovementType.Adjustment);
        return Task.FromResult(new GetStockAdjustmentResponse(
            StockAdjustmentMapper.ToResponse(adjustment), ledger?.Id));
    }
}
