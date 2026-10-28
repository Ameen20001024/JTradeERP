using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Features.Purchasing.Common;
using MediatR;

namespace JTradeERP.Application.Features.Purchasing.PurchaseOrders.GetPurchaseOrderById;

public sealed class GetPurchaseOrderByIdHandler(IApplicationDbContext db)
    : IRequestHandler<GetPurchaseOrderByIdQuery, GetPurchaseOrderByIdResponse>
{
    public Task<GetPurchaseOrderByIdResponse> Handle(GetPurchaseOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = db.PurchaseOrdersWithLines.SingleOrDefault(x => x.Id == request.Id)
            ?? throw new NotFoundException("Purchase order was not found.");
        return Task.FromResult(new GetPurchaseOrderByIdResponse(PurchasingResponseMapper.Map(order)));
    }
}
