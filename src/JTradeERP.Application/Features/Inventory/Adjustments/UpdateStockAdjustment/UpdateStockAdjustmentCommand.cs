using MediatR;
namespace JTradeERP.Application.Features.Inventory.Adjustments.UpdateStockAdjustment;
public sealed record UpdateStockAdjustmentCommand(Guid Id,Guid ProductId,Guid WarehouseId,decimal QuantityDelta,string ReasonCode):IRequest<UpdateStockAdjustmentResponse>;
