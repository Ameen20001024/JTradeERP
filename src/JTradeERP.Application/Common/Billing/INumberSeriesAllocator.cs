namespace JTradeERP.Application.Common.Billing;

public interface INumberSeriesAllocator
{
    Task<string> AllocateAsync(string documentType, CancellationToken cancellationToken);
}
