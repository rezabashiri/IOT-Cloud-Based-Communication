using Broker.Application.MachineManagement.Commands;
using Broker.Domain.DomainEvents;
using Broker.Domain.Entities;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.DTOs.Messages;

namespace Broker.IntegrationTests.MachineManagement;

public class RegisterCommandHandlerTests : BaseTest
{
    private readonly TestEventHandler<MachineAdded> _machineAddedEventHandler = new();

    protected override void OnBuildServiceProvider(IServiceCollection services)
    {
        base.OnBuildServiceProvider(services);
        services.AddTransient<INotificationHandler<MachineAdded>>(_ => _machineAddedEventHandler);
    }

    [Fact]
    public async Task Handler_MachineIsNotExistsInDatabase_MachineShouldAddToDatabase()
    {
        var id = Guid.NewGuid();
        var machine = Machine.Create(
            id,
            "test",
            MachineStatus.Connected);

        await Mediator.Send(new RegisterCommand(new ClientRegister(id, "test")));

        var machineFromDb = await BrokerDbContext.Machines.FirstOrDefaultAsync(m => m.Id == id);
        machineFromDb.Should().NotBeNull();
        machineFromDb!.Name.Should().BeEquivalentTo(machine.Name);
        machineFromDb.Status.Should().Be(machine.Status);
        _machineAddedEventHandler.PublishedEvents.Should().HaveCount(1);
        _machineAddedEventHandler.PublishedEvents.FirstOrDefault().Should().BeOfType<MachineAdded>();
        _machineAddedEventHandler.PublishedEvents.FirstOrDefault()!.MachineId.Should().Be(id);
    }
}