using MediatR;
namespace JTradeERP.Application.Features.Inventory.Adjustments.PostStockAdjustment;
public sealed record PostStockAdjustmentCommand(Guid Id,decimal? UnitCost=null):IRequest<PostStockAdjustmentResponse>;
