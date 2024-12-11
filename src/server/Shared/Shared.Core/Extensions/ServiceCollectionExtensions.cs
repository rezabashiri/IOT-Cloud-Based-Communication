using System.Reflection;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Core.Behaviors;
using Shared.Core.Interfaces.Serialization;
using Shared.Core.Serialization;
using Shared.Core.Settings;

namespace Shared.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedApplication(this IServiceCollection services,
        IConfiguration config)
    {
        services.Configure<CacheSettings>(config.GetSection(nameof(CacheSettings)));
        services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }

    public static IServiceCollection AddSerialization(this IServiceCollection services,
        IConfiguration config)
    {
        services.Configure<SerializationSettings>(config.GetSection(nameof(SerializationSettings)));
        var options = services.GetOptions<SerializationSettings>(nameof(SerializationSettings), config);
        services.AddSingleton<IJsonSerializerSettingsOptions, JsonSerializerSettingsOptions>();
        if (options.UseSystemTextJson)
        {
            services
                .AddSingleton<IJsonSerializer, SystemTextJsonSerializer>()
                .Configure<JsonSerializerSettingsOptions>(
                    configureOptions =>
                    {
                        if (!configureOptions.JsonSerializerOptions.Converters.Any(
                                c =>
                                    c.GetType() == typeof(TimespanJsonConverter)))
                        {
                            configureOptions.JsonSerializerOptions.Converters.Add(new TimespanJsonConverter());
                        }
                    });
        }
        else if (options.UseNewtonsoftJson)
        {
            services
                .AddSingleton<IJsonSerializer, NewtonSoftJsonSerializer>();
        }

        return services;
    }

    public static T GetOptions<T>(this IServiceCollection services,
        string sectionName,
        IConfiguration configuration)
        where T : new()
    {
        var section = configuration.GetSection(sectionName);
        var options = new T();
        section.Bind(options);

        return options;
    }
}