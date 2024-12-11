using Broker.Domain.DomainEvents;
using Shared.Core.Domain;
using Shared.DTOs.Messages;

namespace Broker.Domain.Entities;

public class Machine : BaseEntity
{
    private Machine()
    {
    }

    private Machine(string name,
        MachineStatus status)
    {
        Name = name;
        Status = status;
    }

    public string Name { get; private set; }

    public MachineStatus Status { get; private set; }

    public static Machine Create(Guid id,
        string name,
        MachineStatus status)
    {
        var machine = new Machine(
            name,
            status)
        {
            Id = id
        };

        machine.AddDomainEvent(new MachineAdded(id));

        return machine;
    }

    public void UpdateStatus(MachineStatus status)
    {
        Status = status;
    }
}