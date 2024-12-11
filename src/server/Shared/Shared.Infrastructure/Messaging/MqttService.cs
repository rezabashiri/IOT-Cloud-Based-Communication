using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Exceptions;
using MQTTnet.Protocol;
using Shared.Core.Interfaces.Serialization;
using Shared.Core.Settings;
using Shared.DTOs.Messages;

namespace Shared.Infrastructure.Messaging;

public class MqttService : IBrokerService, IAsyncDisposable
{
    private const short MaxRetries = 3;
    private const int RetryDelayMilliseconds = 1000;
    private readonly IJsonSerializer _jsonSerializer;
    private readonly ILogger<MqttService> _logger;
    private readonly IMqttClient _mqttClient;
    private readonly MqttClientOptions _mqttOptions;
    private short _retryCounter;

    public MqttService(
        IOptions<BrokerSettings> brokerSettings,
        IJsonSerializer jsonSerializer,
        ILogger<MqttService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        if (brokerSettings is null)
        {
            throw new ArgumentNullException(nameof(brokerSettings));
        }

        var brokerSettingsValue = brokerSettings.Value;
        _mqttOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(brokerSettingsValue.Host, brokerSettingsValue.Port)
            .Build();

        _mqttClient = new MqttFactory().CreateMqttClient();
    }

    public async ValueTask DisposeAsync()
    {
        await DisconnectAsync();
        _mqttClient?.Dispose();
    }

    public async Task<bool> PublishAsync<T>(T message,
        string topic,
        CancellationToken cancellationToken = default)
        where T : BaseMessage
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        if (string.IsNullOrWhiteSpace(topic))
        {
            throw new ArgumentException("Topic cannot be null or empty.", nameof(topic));
        }

        await EnsureConnectedAsync(cancellationToken);

        string? serializedMessage = _jsonSerializer.Serialize(message);
        if (string.IsNullOrEmpty(serializedMessage))
        {
            _logger.LogWarning("Serialized message is null or empty, skipping publish.");
            return false;
        }

        var mqttMessage = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(serializedMessage)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();

        try
        {
            var result = await _mqttClient.PublishAsync(mqttMessage, cancellationToken);
            return result.IsSuccess;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message to topic {Topic}.", topic);
            return false;
        }
    }

    public async Task SubscribeAsync(IEnumerable<string> topics,
        Func<MqttApplicationMessageReceivedEventArgs, Task> handler,
        CancellationToken cancellationToken = default)
    {
        if (topics == null || !topics.Any())
        {
            throw new ArgumentException("Topics collection cannot be null or empty.", nameof(topics));
        }

        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        await EnsureConnectedAsync(cancellationToken);

        try
        {
            foreach (string topic in topics)
            {
                await _mqttClient.SubscribeAsync(topic, cancellationToken: cancellationToken);
                _logger.LogInformation("Subscribed to topic {Topic}.", topic);
            }

            _mqttClient.ApplicationMessageReceivedAsync += handler;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to subscribe to topics.");
            throw;
        }
    }

    public async Task EnsureConnectedAsync(CancellationToken cancellationToken)
    {
        if (_mqttClient.IsConnected)
        {
            return;
        }

        _logger.LogInformation("Connecting to MQTT server...");
        _retryCounter = 0;

        while (!_mqttClient.IsConnected && _retryCounter < MaxRetries)
        {
            try
            {
                await _mqttClient.ConnectAsync(_mqttOptions, cancellationToken);
                _logger.LogInformation("Connected to MQTT server.");
                break;
            }
            catch (MqttCommunicationException ex) when (_retryCounter < MaxRetries)
            {
                _retryCounter++;
                _logger.LogWarning(
                    "Failed to connect to MQTT server. Retrying {RetryCounter}/{MaxRetries}...",
                    _retryCounter,
                    MaxRetries);
                await Task.Delay(RetryDelayMilliseconds, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while connecting to MQTT server.");
                throw;
            }
        }
    }

    public async Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_mqttClient.IsConnected)
            {
                return;
            }

            await _mqttClient.DisconnectAsync(new MqttClientDisconnectOptions(), cancellationToken);
            _logger.LogInformation("Disconnected from MQTT server.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error while disconnecting from MQTT server.");
        }
    }
}