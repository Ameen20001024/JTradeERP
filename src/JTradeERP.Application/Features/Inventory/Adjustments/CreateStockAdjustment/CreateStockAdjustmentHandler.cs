using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Inventory;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Inventory;
using MediatR;

namespace JTradeERP.Application.Features.Inventory.Adjustments.CreateStockAdjustment;
public sealed class CreateStockAdjustmentHandler(IApplicationDbContext db,ICurrentUserService actor)
    :IRequestHandler<CreateStockAdjustmentCommand,CreateStockAdjustmentResponse>
{
    public async Task<CreateStockAdjustmentResponse> Handle(CreateStockAdjustmentCommand r,CancellationToken ct)
    {
        if(!db.Products.Any(x => x.Id == r.ProductId))throw new NotFoundException("Product was not found.");
        
        if(!db.Warehouses.Any(x => x.Id == r.WarehouseId))throw new NotFoundException("Warehouse was not found.");
        
        var item = new StockAdjustment(r.ProductId, r.WarehouseId, r.QuantityDelta, r.ReasonCode);
        
        db.Add(item);
        
        SecurityAuditWriter.Add(
            db,
            actor.UserId,
            "StockAdjustmentCreated",
            "StockAdjustment",
            item.Id.ToString(),
            null,
            $"{r.ProductId}|{r.WarehouseId}|{r.QuantityDelta}|{r.ReasonCode}");
        
        await db.SaveChangesAsync(ct);
        
        return new CreateStockAdjustmentResponse(StockAdjustmentMapper.ToResponse(item));
    }
}
