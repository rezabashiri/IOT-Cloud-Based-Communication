using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using Shared.Core.Constants;
using Shared.Core.Interfaces.Serialization;
using Shared.Infrastructure.Messaging;

namespace PLC_Simulator;

public static class Controller
{
    private static readonly Guid _id = Guid.NewGuid();
    private static readonly CancellationTokenSource _cancellationTokenSource = new();

    public static async ValueTask Run(IServiceProvider serviceProvider)
    {
        var connection = serviceProvider.GetRequiredService<IBrokerService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            Console.WriteLine("Starting PLC Simulator ...");
            await connection.EnsureConnectedAsync(_cancellationTokenSource.Token);

            await Task.Delay(1500);

            bool publish =
                await connection.PublishAsync(
                    new ClientRegister(_id, "PLC Simulator"),
                    IOTConstants.PLC_Status_Topic,
                    _cancellationTokenSource.Token);

            Console.WriteLine(
                publish
                    ? $"PLC Simulator registered successfully, simulator id is {_id}"
                    : "Failed to register PLC Simulator");

            await connection.SubscribeAsync([IOTConstants.PLC_Command_Topic], CommandHandler);
            Console.WriteLine("Waiting for commands ...");

            async Task CommandHandler(MqttApplicationMessageReceivedEventArgs arg)
            {
                try
                {
                    using var scope = serviceProvider.CreateScope();
                    var jsonSerializer = scope.ServiceProvider.GetRequiredService<IJsonSerializer>();

                    string? message = arg.ApplicationMessage.ConvertPayloadToString();

                    var baseMessage = jsonSerializer.Deserialize<BaseMessage>(message);

                    Console.WriteLine($"Received message {baseMessage.Type}");
                    if (!baseMessage.ClientId.Equals(_id))
                    {
                        return;
                    }

                    switch (baseMessage.Type)
                    {
                        case nameof(Move):
                            var move = jsonSerializer.Deserialize<Move>(message);
                            Console.WriteLine($"Client {move.ClientId} moved {move.Direction}");
                            await connection.PublishAsync(
                                new ClientCurrentStatus(move.ClientId, MachineStatus.Moving),
                                IOTConstants.PLC_Status_Topic);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            await connection.DisconnectAsync();
        }
    }
}