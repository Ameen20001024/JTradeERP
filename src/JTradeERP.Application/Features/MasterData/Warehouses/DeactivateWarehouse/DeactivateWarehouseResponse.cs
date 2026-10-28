namespace JTradeERP.Application.Features.MasterData.Warehouses.DeactivateWarehouse;
public sealed record DeactivateWarehouseResponse(Guid Id,string WarehouseCode,string Name,bool IsActive);
