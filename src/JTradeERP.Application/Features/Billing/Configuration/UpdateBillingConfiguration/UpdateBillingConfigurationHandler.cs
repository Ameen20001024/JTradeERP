using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Billing;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Billing.Configuration.UpdateBillingConfiguration;

public sealed class UpdateBillingConfigurationHandler
    (IApplicationDbContext db,ICurrentUserService actor):IRequestHandler<UpdateBillingConfigurationCommand,UpdateBillingConfigurationResponse>
{
    public async Task<UpdateBillingConfigurationResponse> Handle(UpdateBillingConfigurationCommand r,CancellationToken ct)
    {
        var x = db.BillingConfigurations
            .SingleOrDefault(y=>y.Id==BillingConfiguration.SingletonId)??throw new NotFoundException("Billing configuration is not initialized.");
        
        var oldCompanyTrn = x.CompanyTrn;
        
        x.SetCompanyTrn(r.CompanyTrn);
        
        var user = actor.UserId??throw new AuthenticationFailureException();
        
        SecurityAuditWriter.Add(
            db,
            user,
            "BillingConfigurationUpdated",
            "BillingConfiguration",
            x.Id.ToString(),
            oldCompanyTrn,
            x.CompanyTrn);
        
        await db.SaveChangesAsync(ct);

        return new UpdateBillingConfigurationResponse(x.CompanyTrn);
    }
}
