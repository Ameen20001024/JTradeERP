using MediatR;
namespace JTradeERP.Application.Features.MasterData.Warehouses.GetWarehouseById;
public sealed record GetWarehouseByIdQuery(Guid Id):IRequest<GetWarehouseByIdResponse>;
