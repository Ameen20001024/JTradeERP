namespace JTradeERP.Application.Features.Billing.NumberSeries.Common;

public sealed record NumberSeriesResponse(
    Guid Id,
    string DocumentType,
    string Prefix,
    long NextNumber,
    int Padding);
