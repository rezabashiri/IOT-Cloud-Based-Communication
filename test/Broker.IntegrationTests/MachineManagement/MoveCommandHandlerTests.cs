using Broker.Application.IOTCommunication;
using Broker.Application.MachineManagement.Commands;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shared.Core.Constants;
using Shared.Core.Exceptions;
using Shared.DTOs.Messages;

namespace Broker.IntegrationTests.MachineManagement;

public class MoveCommandHandlerTests : BaseTest
{
    private readonly Mock<IMessagingService> _machineServiceMock = new();

    protected override void OnBuildServiceProvider(IServiceCollection services)
    {
        base.OnBuildServiceProvider(services);
        services.AddTransient<IMessagingService>(_ => _machineServiceMock.Object);
    }

    [Fact]
    public async Task Handler_DirectionIsValid_ShouldSendToIot()
    {
        var id = Guid.NewGuid();
        var move = new MoveCommand(id, "test");

        await Mediator.Send(move);

        _machineServiceMock.Verify(
            x => x.PublishMessage(
                It.Is<Move>(move1 => move1.ClientId == id && move1.Direction == "test"),
                IOTConstants.PLC_Command_Topic));
    }

    [Fact]
    public async Task Handler_DirectionIsNotValid_ShouldNotSendToIot()
    {
        var id = Guid.NewGuid();
        var move = new MoveCommand(id, string.Empty);

        await Assert.ThrowsAsync<CustomValidationException>(async () => await Mediator.Send(move));

        _machineServiceMock.Verify(
            x => x.PublishMessage(It.IsAny<Move>(), IOTConstants.PLC_Command_Topic),
            Times.Never);
    }
}