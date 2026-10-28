using JTradeERP.Domain.Security;using MediatR;
namespace JTradeERP.Application.Features.Inventory.Adjustments.CreateStockAdjustment;
public static class CreateStockAdjustmentEndPoint{public static void Map(IEndpointRouteBuilder e)=>e.MapPost("/adjustments",async(CreateStockAdjustmentCommand c,ISender s,CancellationToken t)=>Results.Created("/api/inventory/adjustments",await s.Send(c,t))).RequirePermission("Stock.Adjust",Capability.CreateEdit);}
