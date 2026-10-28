using MediatR; using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Exceptions;

namespace JTradeERP.Application.Features.MasterData.Products.ActivateProduct;
public sealed class ActivateProductHandler(IApplicationDbContext db)
    :IRequestHandler<ActivateProductCommand,ActivateProductResponse>
{
    public async Task<ActivateProductResponse> Handle(ActivateProductCommand r,CancellationToken ct)
    {
        var e = db.Products.SingleOrDefault(x=>x.Id==r.Id)
            ??throw new NotFoundException("Product not found.");
        
        e.Activate();
        
        await db.SaveChangesAsync(ct);
        
        return new ActivateProductResponse(e.Id,e.ProductCode,e.Name,e.IsActive);
    }
}
