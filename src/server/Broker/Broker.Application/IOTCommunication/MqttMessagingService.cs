using Broker.Application.MachineManagement.Commands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using Shared.Core.Interfaces.Serialization;
using Shared.DTOs.Messages;
using Shared.Infrastructure.Messaging;

namespace Broker.Application.IOTCommunication;

public class MqttMessagingService : IMessagingService
{
    private readonly IBrokerService _brokerService;
    private readonly IJsonSerializer _jsonSerializer;
    private readonly ILogger<MqttMessagingService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public MqttMessagingService(IBrokerService brokerService,
        IJsonSerializer jsonSerializer,
        ILogger<MqttMessagingService> logger,
        IServiceProvider serviceProvider)
    {
        _brokerService = brokerService;
        _jsonSerializer = jsonSerializer;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public Task Subscribe(List<string> topics)
    {
        return _brokerService.SubscribeAsync(topics, StatusMessageRecieved);
    }

    public Task<bool> PublishMessage<T>(T message,
        string topic) where T : BaseMessage
    {
        return _brokerService.PublishAsync(message, topic);
    }

    public async Task StatusMessageRecieved(MqttApplicationMessageReceivedEventArgs arg)
    {
        try
        {
            string? message = arg.ApplicationMessage.ConvertPayloadToString();
            var baseMessage = _jsonSerializer.Deserialize<BaseMessage>(message);
            using var scope = _serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            _logger.LogInformation($"{baseMessage.Type} has been received");
            switch (baseMessage.Type)
            {
                case nameof(ClientRegister):
                    var registerClient = _jsonSerializer.Deserialize<ClientRegister>(message);
                    await mediator.Send(new RegisterCommand(registerClient));
                    break;
                case nameof(ClientCurrentStatus):
                    var currentStatus = _jsonSerializer.Deserialize<ClientCurrentStatus>(message);
                    await mediator.Send(new UpdateCommand(currentStatus.ClientId, currentStatus.MachineStatus));
                    break;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }
    }
}