using Broker.Application.IOTCommunication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Core.Constants;

namespace Broker.Application.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseApplication(this IApplicationBuilder app)
    {
        StartReceivingMessages(app.ApplicationServices);

        return app;
    }

    private static void StartReceivingMessages(IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.CreateScope().ServiceProvider;
        var messageHandler = scope.GetRequiredService<IMessagingService>();
        var logger = scope.GetRequiredService<ILogger<MqttMessagingService>>();
        try
        {
            messageHandler.Subscribe([IOTConstants.PLC_Status_Topic]).GetAwaiter().GetResult();
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
        }
    }
}