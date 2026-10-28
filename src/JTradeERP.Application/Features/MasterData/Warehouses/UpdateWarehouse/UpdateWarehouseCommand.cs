using MediatR;
namespace JTradeERP.Application.Features.MasterData.Warehouses.UpdateWarehouse;
public sealed record UpdateWarehouseCommand(Guid Id,string WarehouseCode,string Name):IRequest<UpdateWarehouseResponse>;
