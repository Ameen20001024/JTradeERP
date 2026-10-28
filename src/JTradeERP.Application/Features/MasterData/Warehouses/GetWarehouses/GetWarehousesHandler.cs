using MediatR;using JTradeERP.Application.Common.Persistence;
namespace JTradeERP.Application.Features.MasterData.Warehouses.GetWarehouses;
public sealed class GetWarehousesHandler(IApplicationDbContext db):IRequestHandler<GetWarehousesQuery,IReadOnlyList<GetWarehousesResponse>>{public Task<IReadOnlyList<GetWarehousesResponse>> Handle(GetWarehousesQuery r,CancellationToken ct)=>Task.FromResult<IReadOnlyList<GetWarehousesResponse>>(db.Warehouses.OrderBy(x=>x.WarehouseCode).Select(e=>new GetWarehousesResponse(e.Id,e.WarehouseCode,e.Name,e.IsActive)).ToList());}
