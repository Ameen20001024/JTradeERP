using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Billing.NumberSeries.CreateNumberSeries;

public static class CreateNumberSeriesEndPoint
{
    public static void Map(IEndpointRouteBuilder e) => e.MapPost(
        "/number-series",
        async(CreateNumberSeriesCommand c, ISender s, CancellationToken ct) => 
        
        Results.Ok(await s.Send(c,ct)))
        
        .RequirePermission("Admin.NumberSeries.Manage",Capability.CreateEdit);
}
