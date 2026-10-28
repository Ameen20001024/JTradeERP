using MediatR;

namespace JTradeERP.Application.Features.Billing.Configuration.GetBillingConfiguration;

public sealed record GetBillingConfigurationQuery:IRequest<GetBillingConfigurationResponse>;
