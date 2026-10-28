using MediatR;
namespace JTradeERP.Application.Features.MasterData.Warehouses.DeactivateWarehouse;
public sealed record DeactivateWarehouseCommand(Guid Id):IRequest<DeactivateWarehouseResponse>;
