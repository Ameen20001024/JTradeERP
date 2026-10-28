using MediatR;
namespace JTradeERP.Application.Features.Inventory.Adjustments.GetStockAdjustments;
public sealed record GetStockAdjustmentsQuery(Guid? ProductId=null,Guid? WarehouseId=null,string? Status=null,int Page=1,int PageSize=50):IRequest<GetStockAdjustmentsResponse>;
