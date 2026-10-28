using MediatR;

namespace JTradeERP.Application.Features.Billing.Configuration.UpdateBillingConfiguration;

public sealed record UpdateBillingConfigurationCommand(string? CompanyTrn):IRequest<UpdateBillingConfigurationResponse>;
