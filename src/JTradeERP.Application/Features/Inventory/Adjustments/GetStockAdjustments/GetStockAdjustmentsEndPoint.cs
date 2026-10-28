using JTradeERP.Domain.Security;using MediatR;
namespace JTradeERP.Application.Features.Inventory.Adjustments.GetStockAdjustments;
public static class GetStockAdjustmentsEndPoint{public static void Map(IEndpointRouteBuilder e)=>e.MapGet("/adjustments",async(Guid? productId,Guid? warehouseId,string? status,int? page,int? pageSize,ISender s,CancellationToken t)=>Results.Ok(await s.Send(new GetStockAdjustmentsQuery(productId,warehouseId,status,page??1,pageSize??50),t))).RequirePermission("Stock.View",Capability.View);}
