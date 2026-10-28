using MediatR;
namespace JTradeERP.Application.Features.MasterData.Warehouses.GetWarehouses;
public sealed record GetWarehousesQuery:IRequest<IReadOnlyList<GetWarehousesResponse>>;
