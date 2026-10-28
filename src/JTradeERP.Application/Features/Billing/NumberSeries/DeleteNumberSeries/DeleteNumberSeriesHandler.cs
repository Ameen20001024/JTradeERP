using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using MediatR;
namespace JTradeERP.Application.Features.Billing.NumberSeries.DeleteNumberSeries;
public sealed class DeleteNumberSeriesHandler(IApplicationDbContext db)
    :IRequestHandler<DeleteNumberSeriesCommand,DeleteNumberSeriesResponse>
{
    public async Task<DeleteNumberSeriesResponse> Handle(DeleteNumberSeriesCommand r,CancellationToken ct)
    {
        var x = db.NumberSeries.SingleOrDefault(x=>x.Id==r.Id)
            ??throw new NotFoundException("Number series not found.");
        
        db.Remove(x);
        
        await db.SaveChangesAsync(ct);
        
        return new(x.Id,true);
    }
}
