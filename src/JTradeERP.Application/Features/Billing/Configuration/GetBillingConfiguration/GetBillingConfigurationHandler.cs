using JTradeERP.Application.Common.Persistence;
using JTradeERP.Domain.Billing;
using MediatR;

namespace JTradeERP.Application.Features.Billing.Configuration.GetBillingConfiguration;

public sealed class GetBillingConfigurationHandler
    (IApplicationDbContext db):IRequestHandler<GetBillingConfigurationQuery,GetBillingConfigurationResponse>
{
    public Task<GetBillingConfigurationResponse> Handle(GetBillingConfigurationQuery r,CancellationToken ct) => 
        Task.FromResult(new GetBillingConfigurationResponse(
            db.BillingConfigurations
            .SingleOrDefault(x=>x.Id==BillingConfiguration.SingletonId)?
            .CompanyTrn));
}
