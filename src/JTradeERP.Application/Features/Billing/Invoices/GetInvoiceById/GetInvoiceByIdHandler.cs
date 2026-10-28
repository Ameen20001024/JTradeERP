using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Features.Billing.Common;
using MediatR;

namespace JTradeERP.Application.Features.Billing.Invoices.GetInvoiceById;

public sealed class GetInvoiceByIdHandler
    (IApplicationDbContext db):IRequestHandler<GetInvoiceByIdQuery,GetInvoiceByIdResponse>
{
    public Task<GetInvoiceByIdResponse> Handle(GetInvoiceByIdQuery r,CancellationToken ct)
    {
        var x = db.InvoicesWithLines.SingleOrDefault(x=>x.Id==r.Id)
            ??throw new NotFoundException("Invoice not found.");
        
        return Task.FromResult(new GetInvoiceByIdResponse(BillingResponseMapper.Map(x)));
    }
}
