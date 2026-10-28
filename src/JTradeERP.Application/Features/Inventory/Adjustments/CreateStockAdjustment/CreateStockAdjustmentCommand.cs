using MediatR;
namespace JTradeERP.Application.Features.Inventory.Adjustments.CreateStockAdjustment;
public sealed record CreateStockAdjustmentCommand(Guid ProductId,Guid WarehouseId,decimal QuantityDelta,string ReasonCode):IRequest<CreateStockAdjustmentResponse>;
