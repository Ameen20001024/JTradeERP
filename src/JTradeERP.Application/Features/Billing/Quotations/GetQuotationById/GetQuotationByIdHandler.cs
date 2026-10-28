using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Features.Billing.Common;
using MediatR;

namespace JTradeERP.Application.Features.Billing.Quotations.GetQuotationById;
public sealed class GetQuotationByIdHandler(IApplicationDbContext db):IRequestHandler<GetQuotationByIdQuery,GetQuotationByIdResponse>
{
    public Task<GetQuotationByIdResponse> Handle(GetQuotationByIdQuery r,CancellationToken ct)
    {
        var x = db.QuotationsWithLines.SingleOrDefault(x=>x.Id==r.Id)??throw new NotFoundException("Quotation not found.");
        return Task.FromResult(new GetQuotationByIdResponse(BillingResponseMapper.Map(x)));
    }
}
