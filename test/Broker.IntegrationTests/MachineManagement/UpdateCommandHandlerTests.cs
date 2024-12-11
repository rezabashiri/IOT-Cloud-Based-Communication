using Broker.Application.MachineManagement.Commands;
using Broker.Domain.Entities;
using FluentAssertions;
using Shared.DTOs.Messages;

namespace Broker.IntegrationTests.MachineManagement;

public class UpdateCommandHandlerTests : BaseTest
{
    [Fact]
    public async Task Handler_MachineIsNotExistsInDatabase_ErrorShouldBeReturned()
    {
        var id = Guid.NewGuid();
        var machine = Machine.Create(id, "test", MachineStatus.Connected);

        var result = await Mediator.Send(new UpdateCommand(id, MachineStatus.Moving));

        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
    }

    [Fact]
    public async Task Handler_MachineIsExistsInDatabase_MachineShouldBeUpdated()
    {
        var id = Guid.NewGuid();
        var machine = Machine.Create(id, "test", MachineStatus.Connected);
        BrokerDbContext.Machines.Add(machine);
        await BrokerDbContext.SaveChangesAsync();

        await Mediator.Send(new UpdateCommand(id, MachineStatus.Moving));

        BrokerDbContext.Machines.Should().Contain(machine);
        machine.Status.Should().Be(MachineStatus.Moving);
    }
}