using Broker.Domain.DomainEvents;
using Broker.Domain.Entities;
using FluentAssertions;
using Shared.DTOs.Messages;

namespace Broker.UnitTests.Entities;

public class MachineUnitTests
{
    [Fact]
    public void CreateMachine_ValidParameters_ShouldHaveDomainEventAndCorrectProperties()
    {
        var machine = Machine.Create(Guid.NewGuid(), "test", MachineStatus.Connected);

        machine.DomainEvents.Should().HaveCount(1);
        machine.DomainEvents.FirstOrDefault().Should().BeOfType<MachineAdded>();
        machine.Status.Should().Be(MachineStatus.Connected);
    }
}