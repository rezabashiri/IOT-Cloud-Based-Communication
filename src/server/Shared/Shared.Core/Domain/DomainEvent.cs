namespace Shared.Core.Domain;

public abstract record DomainEvent : Event
{
    protected DomainEvent(Guid id)
    {
        AggregateId = id;
    }
}