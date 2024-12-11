using Shared.Core.Domain;

namespace Broker.Domain.DomainEvents;

public record MachineAdded(Guid MachineId) : DomainEvent(MachineId);