using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shared.Core.Extensions;

namespace Broker.UnitTests;

public abstract class BaseTest
{
    protected BaseTest()
    {
        ServiceProvider = MakeServiceProvider(new ServiceCollection());
    }

    protected IServiceProvider ServiceProvider { get; }
    protected Mock<IMediator> MediatorMock { get; set; } = new();

    private IServiceProvider MakeServiceProvider(IServiceCollection services)
    {
        var configuration = BuildConfiguration();
        services.AddSingleton(configuration);
        services.AddSerialization(configuration);
        services.AddSingleton<IMediator>(_ => MediatorMock.Object);
        return services.BuildServiceProvider();
    }

    private IConfiguration BuildConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(Path.GetFullPath("../../../../../src/server/Broker/Broker.Interface/appsettings.json"));

        return configuration.Build();
    }
}