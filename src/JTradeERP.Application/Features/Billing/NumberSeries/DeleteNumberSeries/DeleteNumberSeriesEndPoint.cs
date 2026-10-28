using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Billing.NumberSeries.DeleteNumberSeries;

public static class DeleteNumberSeriesEndPoint
{
    public static void Map(IEndpointRouteBuilder e)=>e.MapDelete(
        "/number-series/{id:guid}",
        async(Guid id,ISender s,CancellationToken ct) => 
        
        Results.Ok(await s.Send(new DeleteNumberSeriesCommand(id),ct)))
        
        .RequirePermission("Admin.NumberSeries.Manage",Capability.CreateEdit);
}
