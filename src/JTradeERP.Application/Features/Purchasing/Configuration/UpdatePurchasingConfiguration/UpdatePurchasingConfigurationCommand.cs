using MediatR;

namespace JTradeERP.Application.Features.Purchasing.Configuration.UpdatePurchasingConfiguration;

public sealed record UpdatePurchasingConfigurationCommand(bool AllowOverReceipt) : IRequest<UpdatePurchasingConfigurationResponse>;
