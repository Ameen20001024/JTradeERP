using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using MediatR;
using JTradeERP.Domain.Purchasing;

namespace JTradeERP.Application.Features.Purchasing.Configuration.GetPurchasingConfiguration;

public sealed class GetPurchasingConfigurationHandler(IApplicationDbContext db)
    : IRequestHandler<GetPurchasingConfigurationQuery, GetPurchasingConfigurationResponse>
{
    public Task<GetPurchasingConfigurationResponse> Handle(GetPurchasingConfigurationQuery request, CancellationToken cancellationToken)
    {
        var configuration = db.PurchasingConfigurations.SingleOrDefault(x => x.Id == PurchasingConfiguration.SingletonId)
            ?? throw new ConflictException("Purchasing configuration is missing.");
        return Task.FromResult(new GetPurchasingConfigurationResponse(configuration.AllowOverReceipt,
            Convert.ToBase64String(configuration.RowVersion)));
    }
}
