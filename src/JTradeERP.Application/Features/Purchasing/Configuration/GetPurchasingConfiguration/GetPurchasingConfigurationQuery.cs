using MediatR;

namespace JTradeERP.Application.Features.Purchasing.Configuration.GetPurchasingConfiguration;

public sealed record GetPurchasingConfigurationQuery : IRequest<GetPurchasingConfigurationResponse>;
