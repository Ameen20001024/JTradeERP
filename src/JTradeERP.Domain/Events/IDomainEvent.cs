namespace JTradeERP.Domain.Events;

public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}
