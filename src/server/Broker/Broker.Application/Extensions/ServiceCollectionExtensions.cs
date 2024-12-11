using System.Reflection;
using Broker.Application.IOTCommunication;
using Broker.Infrastructure.Extensions;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure.Extensions;

namespace Broker.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBrokerApplication(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection.AddMediatR(
            serviceConfiguration =>
                serviceConfiguration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        serviceCollection.AddSingleton<IMessagingService, MqttMessagingService>();
        serviceCollection.AddMqttBroker(configuration);
        serviceCollection.AddAutoMapper(Assembly.GetExecutingAssembly());
        serviceCollection.AddInfrastructure(configuration);
        serviceCollection.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return serviceCollection;
    }
}