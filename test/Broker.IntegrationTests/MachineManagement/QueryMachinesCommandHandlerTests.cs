using Broker.Application.MachineManagement.Queries;
using Broker.Domain.Entities;
using Broker.IntegrationTests;
using FluentAssertions;
using Shared.DTOs.Messages;

public class QueryMachinesCommandHandlerTests : BaseTest
{
    [Fact]
    public async Task Handle_WhenMachinesExist_ShouldReturnPaginatedResultWithData()
    {
        var machine1 = Machine.Create(Guid.NewGuid(), "Machine 1", MachineStatus.Connected);
        var machine2 = Machine.Create(Guid.NewGuid(), "Machine 2", MachineStatus.Moving);

        BrokerDbContext.Machines.Add(machine1);
        BrokerDbContext.Machines.Add(machine2);
        await BrokerDbContext.SaveChangesAsync();


        var result = await Mediator.Send(new QueryMachines());


        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data.First().Name.Should().Be("Machine 1");
    }

    [Fact]
    public async Task Handle_WhenNoMachinesExist_ShouldReturnFailResult()
    {
        var result = await Mediator.Send(new QueryMachines());

        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.Data.Should().BeEmpty();
    }
}