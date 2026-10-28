using MediatR;
namespace JTradeERP.Application.Features.Inventory.Adjustments.ReverseStockAdjustment;
public sealed record ReverseStockAdjustmentCommand(Guid Id):IRequest<ReverseStockAdjustmentResponse>;
