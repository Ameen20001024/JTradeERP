using MediatR;

namespace JTradeERP.Application.Features.Billing.NumberSeries.CreateNumberSeries;

public sealed record CreateNumberSeriesCommand(
    string DocumentType,
    string Prefix,
    long NextNumber,
    int Padding):IRequest<CreateNumberSeriesResponse>;
