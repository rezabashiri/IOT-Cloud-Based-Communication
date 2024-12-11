using Shared.DTOs.Messages;

namespace Broker.Application.IOTCommunication;

public interface IMessagingService
{
    Task Subscribe(List<string> topics);

    Task<bool> PublishMessage<T>(T message,
        string topic) where T : BaseMessage;
}