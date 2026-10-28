using MediatR;
namespace JTradeERP.Application.Features.Inventory.Adjustments.GetStockAdjustment;
public sealed record GetStockAdjustmentQuery(Guid Id):IRequest<GetStockAdjustmentResponse>;
