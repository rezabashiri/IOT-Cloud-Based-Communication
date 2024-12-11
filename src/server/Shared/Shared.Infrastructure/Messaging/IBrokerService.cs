using MQTTnet.Client;
using Shared.DTOs.Messages;

namespace Shared.Infrastructure.Messaging;

public interface IBrokerService
{
    Task<bool> PublishAsync<T>(T message,
        string topic,
        CancellationToken cancellationToken = default)
        where T : BaseMessage;

    Task SubscribeAsync(IEnumerable<string> topics,
        Func<MqttApplicationMessageReceivedEventArgs, Task> handler,
        CancellationToken cancellationToken = default);

    Task EnsureConnectedAsync(CancellationToken cancellationToken);
    Task DisconnectAsync(CancellationToken cancellationToken = default);
}