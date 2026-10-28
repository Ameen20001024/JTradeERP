namespace JTradeERP.Application.Common.Persistence;

public interface IApplicationTransaction : IAsyncDisposable
{
    Task CommitAsync(CancellationToken cancellationToken);
}
