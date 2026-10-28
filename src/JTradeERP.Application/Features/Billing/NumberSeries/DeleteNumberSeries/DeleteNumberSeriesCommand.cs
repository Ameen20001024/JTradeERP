using MediatR;

namespace JTradeERP.Application.Features.Billing.NumberSeries.DeleteNumberSeries;

public sealed record DeleteNumberSeriesCommand(Guid Id):IRequest<DeleteNumberSeriesResponse>;
