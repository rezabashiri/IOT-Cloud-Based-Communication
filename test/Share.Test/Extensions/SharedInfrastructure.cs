using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure.Extensions;

namespace Share.Test.Extensions
{
    public static class SharedInfrastructure
    {
        public static IServiceCollection AddsSharedInfrastructureForTestAssemblies(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSharedInfrastructure(configuration);

            return services;
        }

    }
}