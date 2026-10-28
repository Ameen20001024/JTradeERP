namespace JTradeERP.Application.Features.MasterData.Warehouses.GetWarehouseById;
public sealed record GetWarehouseByIdResponse(Guid Id,string WarehouseCode,string Name,bool IsActive);
