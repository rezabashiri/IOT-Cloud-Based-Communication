using Broker.Application.IOTCommunication;
using Broker.Application.MachineManagement.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Packets;
using MQTTnet.Protocol;
using Shared.Core.Constants;
using Shared.Core.Interfaces.Serialization;
using Shared.DTOs.Messages;
using Shared.Infrastructure.Messaging;

namespace Broker.UnitTests.IOTCommunication;

public class MqttMessagingServiceTests : BaseTest
{
    [Fact]
    public async Task MessageReceived_RegisterClientMessage_ShouldDeserializeAndDispatchRegisterClientCommand()
    {
        var jsonSerializer = ServiceProvider.GetRequiredService<IJsonSerializer>();
        var payLoad = jsonSerializer.Serialize(new ClientRegister(Guid.NewGuid(), "test"));
        var messageHandler = new MqttMessagingService(
            new Mock<IBrokerService>().Object,
            jsonSerializer,
            new Mock<ILogger<MqttMessagingService>>().Object,
            ServiceProvider);

        var statusMessage = new MqttApplicationMessageBuilder()
            .WithTopic(IOTConstants.PLC_Status_Topic)
            .WithPayload(payLoad)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();
        await messageHandler.StatusMessageRecieved(
            new MqttApplicationMessageReceivedEventArgs("test", statusMessage, new MqttPublishPacket(), null));

        MediatorMock
            .Verify(mediator => mediator.Send(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task MessageReceived_CurrentStatusMessage_ShouldDeserializeAndDispatchUpdateCommand()
    {
        var jsonSerializer = ServiceProvider.GetRequiredService<IJsonSerializer>();
        var payLoad = jsonSerializer.Serialize(new ClientCurrentStatus(Guid.NewGuid(), MachineStatus.Moving));
        var messageHandler = new MqttMessagingService(
            new Mock<IBrokerService>().Object,
            jsonSerializer,
            new Mock<ILogger<MqttMessagingService>>().Object,
            ServiceProvider);

        var statusMessage = new MqttApplicationMessageBuilder()
            .WithTopic(IOTConstants.PLC_Status_Topic)
            .WithPayload(payLoad)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();
        await messageHandler.StatusMessageRecieved(
            new MqttApplicationMessageReceivedEventArgs("test", statusMessage, new MqttPublishPacket(), null));

        MediatorMock
            .Verify(mediator => mediator.Send(It.IsAny<UpdateCommand>(), It.IsAny<CancellationToken>()));
    }
}