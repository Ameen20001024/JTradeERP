using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Features.Billing.Common;
using MediatR;

namespace JTradeERP.Application.Features.Billing.SalesOrders.GetSalesOrderById;

public sealed class GetSalesOrderByIdHandler(IApplicationDbContext db):IRequestHandler<GetSalesOrderByIdQuery,GetSalesOrderByIdResponse>
{
    public Task<GetSalesOrderByIdResponse> Handle(GetSalesOrderByIdQuery r,CancellationToken ct)
    {
        var x = db.SalesOrdersWithLines.SingleOrDefault(x=>x.Id==r.Id)??throw new NotFoundException("Sales order not found.");
        return Task.FromResult(new GetSalesOrderByIdResponse(BillingResponseMapper.Map(x)));
    }
}
