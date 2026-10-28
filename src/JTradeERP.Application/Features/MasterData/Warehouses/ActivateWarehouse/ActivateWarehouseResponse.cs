namespace JTradeERP.Application.Features.MasterData.Warehouses.ActivateWarehouse;
public sealed record ActivateWarehouseResponse(Guid Id,string WarehouseCode,string Name,bool IsActive);
