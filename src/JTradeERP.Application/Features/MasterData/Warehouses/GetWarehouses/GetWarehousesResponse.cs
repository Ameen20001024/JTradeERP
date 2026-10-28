namespace JTradeERP.Application.Features.MasterData.Warehouses.GetWarehouses;
public sealed record GetWarehousesResponse(Guid Id,string WarehouseCode,string Name,bool IsActive);
