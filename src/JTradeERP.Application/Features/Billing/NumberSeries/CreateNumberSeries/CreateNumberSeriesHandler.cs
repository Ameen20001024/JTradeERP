using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Features.Billing.NumberSeries.Common;
using MediatR;

namespace JTradeERP.Application.Features.Billing.NumberSeries.CreateNumberSeries;

public sealed class CreateNumberSeriesHandler(IApplicationDbContext db)
    :IRequestHandler<CreateNumberSeriesCommand,CreateNumberSeriesResponse>
{
    public async Task<CreateNumberSeriesResponse> Handle(CreateNumberSeriesCommand r,CancellationToken ct)
    {
        if(db.NumberSeries.Any(x=>x.DocumentType==r.DocumentType.Trim()))
            throw new ConflictException("Document number series already exists.");
        
        var x = new JTradeERP.Domain.Billing.NumberSeries(r.DocumentType,r.Prefix,r.NextNumber,r.Padding);
        
        db.Add(x);
        
        await db.SaveChangesAsync(ct);
        
        return new CreateNumberSeriesResponse(new NumberSeriesResponse(x.Id,x.DocumentType,x.Prefix,x.NextNumber,x.Padding));
    }
}
