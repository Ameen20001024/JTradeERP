using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Inventory;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Inventory;
using MediatR;

namespace JTradeERP.Application.Features.Inventory.Adjustments.PostStockAdjustment;
public sealed class PostStockAdjustmentHandler(IApplicationDbContext db,IStockPostingService stock,ICurrentUserService actor,IClock clock):
    IRequestHandler<PostStockAdjustmentCommand,PostStockAdjustmentResponse>
{
    public async Task<PostStockAdjustmentResponse> Handle(PostStockAdjustmentCommand r,CancellationToken ct)
    {
        var x = db.StockAdjustments.SingleOrDefault(x=>x.Id==r.Id)
            ??throw new NotFoundException("Stock adjustment was not found.");
        
        var existing = db.StockLedgerEntries
            .SingleOrDefault(l => l.SourceDocumentType == "StockAdjustment" 
            && l.SourceDocumentId == x.Id 
            && l.MovementType == StockMovementType.Adjustment);
        
        if(x.Status != InventoryDocumentStatus.Draft && existing is not null)
            return new PostStockAdjustmentResponse(StockAdjustmentMapper.ToResponse(x), existing.Id);
        
        if(x.Status != InventoryDocumentStatus.Draft)
            throw new ConflictException("Only draft adjustments can be posted.");
        
        var actorId = actor.UserId ?? throw new AuthenticationFailureException();
        
        stock.EnsureActiveReferences(x.ProductId,x.WarehouseId);
        
        decimal valuationCost;

        if (x.QuantityDelta > 0)
        {
            if (!r.UnitCost.HasValue) throw new ValidationFailureException(new Dictionary<string, string[]>
            {
                {
                    "UnitCost",["Positive adjustments require a unit cost."]
                }
            });

            valuationCost = r.UnitCost.Value;
        }

        else
        {
            var b = stock.GetOrCreateBalance(x.ProductId, x.WarehouseId);

            valuationCost = b.WeightedAverageCost;
        }

        StockLedgerEntry ledger;

        if (x.QuantityDelta > 0)

            ledger = stock.Receive(x.ProductId,
                x.WarehouseId,
                x.QuantityDelta,
                valuationCost,
                "StockAdjustment",
                x.Id,
                null,
                StockMovementType.Adjustment,
                actorId,
                InventoryPostingMetadata.CorrelationId);

        else
            ledger = stock.Issue(x.ProductId,
                x.WarehouseId,
                Math.Abs(x.QuantityDelta),
                "StockAdjustment",
                x.Id,
                null,
                StockMovementType.Adjustment,
                actorId,
                InventoryPostingMetadata.CorrelationId);
        

        x.Post(valuationCost, clock.UtcNow,actorId);
        
        SecurityAuditWriter.Add(db,
            actorId,
            "StockAdjustmentPosted",
            "StockAdjustment",
            x.Id.ToString(),
            "Draft",
            $"{x.QuantityDelta}|{valuationCost}");
        
        await db.SaveChangesAsync(ct);
        
        return new PostStockAdjustmentResponse(StockAdjustmentMapper.ToResponse(x), ledger.Id);
    }
}
