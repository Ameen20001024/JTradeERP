using JTradeERP.Domain.Security;
using MediatR;
namespace JTradeERP.Application.Features.Inventory.StockLedger.GetStockLedger;
public static class GetStockLedgerEndPoint
{
    public static void Map(IEndpointRouteBuilder e)=>e.MapGet("/stock-ledger",async(Guid? productId,Guid? warehouseId,DateTimeOffset? from,DateTimeOffset? to,string? sourceDocumentType,int? page,int? pageSize,ISender s,CancellationToken ct)=>
        Results.Ok(await s.Send(new GetStockLedgerQuery(productId,warehouseId,from,to,sourceDocumentType,page??1,pageSize??100),ct))).RequirePermission("Stock.View",Capability.View);
}
