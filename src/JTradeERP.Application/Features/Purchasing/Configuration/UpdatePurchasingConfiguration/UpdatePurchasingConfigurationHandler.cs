using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Purchasing;
using MediatR;

namespace JTradeERP.Application.Features.Purchasing.Configuration.UpdatePurchasingConfiguration;

public sealed class UpdatePurchasingConfigurationHandler(IApplicationDbContext db, ICurrentUserService actor)
    : IRequestHandler<UpdatePurchasingConfigurationCommand, UpdatePurchasingConfigurationResponse>
{
    public async Task<UpdatePurchasingConfigurationResponse> Handle(UpdatePurchasingConfigurationCommand request, CancellationToken cancellationToken)
    {
        var configuration = db.PurchasingConfigurations.SingleOrDefault(x => x.Id == PurchasingConfiguration.SingletonId)
            ?? throw new ConflictException("Purchasing configuration is missing.");
        var userId = actor.UserId ?? throw new AuthenticationFailureException();
        var old = configuration.AllowOverReceipt.ToString();
        configuration.SetAllowOverReceipt(request.AllowOverReceipt);
        SecurityAuditWriter.Add(db, userId, "PurchasingConfigurationUpdated", "PurchasingConfiguration",
            configuration.Id.ToString(), old, request.AllowOverReceipt.ToString());
        await db.SaveChangesAsync(cancellationToken);
        return new(configuration.AllowOverReceipt, Convert.ToBase64String(configuration.RowVersion));
    }
}
