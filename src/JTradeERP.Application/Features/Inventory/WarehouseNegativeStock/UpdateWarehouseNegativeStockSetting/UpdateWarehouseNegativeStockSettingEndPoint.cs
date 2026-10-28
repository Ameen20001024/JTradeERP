using JTradeERP.Domain.Security;using MediatR;
namespace JTradeERP.Application.Features.Inventory.WarehouseNegativeStock.UpdateWarehouseNegativeStockSetting;
public static class UpdateWarehouseNegativeStockSettingEndPoint
{
    // Existing ARP-01 configuration policy; no new permission code is introduced.
    public static void Map(IEndpointRouteBuilder e)=>e.MapPut("/warehouses/{id:guid}/negative-stock",async(Guid id,UpdateWarehouseNegativeStockSettingCommand c,ISender s,CancellationToken t)=>Results.Ok(await s.Send(c with{Id=id},t))).RequirePermission("Admin.Configuration.Manage",Capability.Full);
}
