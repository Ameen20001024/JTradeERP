using MediatR;
namespace JTradeERP.Application.Features.MasterData.Warehouses.ActivateWarehouse;
public sealed record ActivateWarehouseCommand(Guid Id):IRequest<ActivateWarehouseResponse>;
