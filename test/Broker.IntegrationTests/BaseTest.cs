using Broker.Application.Extensions;
using Broker.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Share.Test.Extensions;
using Shared.Test.Extensions;

namespace Broker.IntegrationTests;

public abstract class BaseTest
{
    protected BaseTest()
    {
        ServiceProvider = BuildServiceProvider(new ServiceCollection());
    }

    protected IBrokerDbContext BrokerDbContext => ServiceProvider.GetService<IBrokerDbContext>()!;
    protected IMediator Mediator => ServiceProvider.GetService<IMediator>()!;

    protected IServiceProvider ServiceProvider { get; }

    protected virtual void OnBuildServiceProvider(IServiceCollection services)
    {
    }

    private IServiceProvider BuildServiceProvider(IServiceCollection services)
    {
        var configuration = BuildConfiguration();
        services.AddSingleton(configuration);
        services.AddSharedCoreServicesForTestAssemblies(configuration);
        services.AddsSharedInfrastructureForTestAssemblies(configuration);
        services.AddBrokerApplication(configuration);
        OnBuildServiceProvider(services);
        return services.BuildServiceProvider();
    }

    private IConfiguration BuildConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(Path.GetFullPath("../../../../../src/server/Broker/Broker.Interface/appsettings.json"))
            .AddInMemoryCollection(
                new List<KeyValuePair<string, string>>
                {
                    new(
                        "PersistenceSettings:UseSqlite",
                        "false"),
                    new(
                        "PersistenceSettings:UseInMemory",
                        "true")
                });

        return configuration.Build();
    }
}