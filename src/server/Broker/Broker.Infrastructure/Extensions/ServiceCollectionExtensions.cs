using Broker.Domain.Abstractions;
using Broker.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure.Persistence;

namespace Broker.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDatabaseContext<BrokerDbContext>(configuration);

        services.AddScoped<IBrokerDbContext>(provider => provider.GetService<BrokerDbContext>() !);

        return services;
    }
}