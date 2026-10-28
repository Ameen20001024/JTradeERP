using MediatR;
namespace JTradeERP.Application.Features.MasterData.Warehouses.CreateWarehouse;
public sealed record CreateWarehouseCommand(string WarehouseCode,string Name):IRequest<CreateWarehouseResponse>;
