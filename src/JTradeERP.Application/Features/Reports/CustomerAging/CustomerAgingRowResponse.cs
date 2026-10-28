namespace JTradeERP.Application.Features.Reports.CustomerAging;

public sealed record CustomerAgingRowResponse(
    Guid CustomerId,
    string CustomerCode,
    string CustomerName,
    DateOnly AsOfDate,
    decimal Current,
    decimal Days1To30,
    decimal Days31To60,
    decimal Days61To90,
    decimal Days91Plus,
    decimal TotalOutstanding);
