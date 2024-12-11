using System.Runtime.CompilerServices;
using System.Text;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Shared.Core.Extensions;
using Shared.Core.Settings;
using Shared.Infrastructure.Interceptors;
using Shared.Infrastructure.Messaging;
using Shared.Infrastructure.Middlewares;
using Shared.Infrastructure.Swagger.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;

[assembly: InternalsVisibleTo("Broker.Interface")]
[assembly: InternalsVisibleTo("Shared.Test")]

namespace Shared.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddSharedInfrastructure(this IServiceCollection services,
        IConfiguration config)
    {
        services.AddPersistenceSettings(config);

        services.AddApiVersioning(
            o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
            });
        services.AddTransient<IValidatorInterceptor, ValidatorInterceptor>();
        services.AddLocalization(options => { options.ResourcesPath = "Resources"; });
        services.AddLogging(options => options.AddConsole());
        services.AddGlobalExceptionHandler();
        services.AddSwaggerDocumentation();
        services.AddDistributedMemoryCache();
        return services;
    }

    public static IServiceCollection AddGlobalExceptionHandler(this IServiceCollection services)
    {
        services.AddSingleton<GlobalExceptionHandler>();
        return services;
    }

    private static IServiceCollection AddPersistenceSettings(this IServiceCollection services,
        IConfiguration config)
    {
        return services
            .Configure<PersistenceSettings>(config.GetSection(nameof(PersistenceSettings)));
    }

    public static IServiceCollection AddMqttBroker(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<BrokerSettings>(configuration.GetSection(nameof(BrokerSettings)));
        return services.AddSingleton<IBrokerService, MqttService>();
    }

    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        return services.AddSwaggerGen(
            options =>
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (!assembly.IsDynamic)
                    {
                        string xmlFile = $"{assembly.GetName().Name}.xml";
                        string xmlPath = Path.Combine(baseDirectory, xmlFile);
                        if (File.Exists(xmlPath))
                        {
                            options.IncludeXmlComments(xmlPath);
                            options.IncludeGrpcXmlComments(xmlPath);
                        }
                    }
                }

                options.AddSwaggerDocs();

                options.OperationFilter<RemoveVersionFromParameterFilter>();
                options.DocumentFilter<ReplaceVersionWithExactValueInPathFilter>();
                options.OperationFilter<SwaggerExcludeFilter>();

                options.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        Description =
                            "Input your Bearer token in this format - Bearer {your token here} to access this API"
                    });
                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                                Scheme = "Bearer",
                                Name = "Bearer",
                                In = ParameterLocation.Header
                            },
                            new List<string>()
                        }
                    });
                options.MapType<TimeSpan>(
                    () => new OpenApiSchema
                    {
                        Type = "string",
                        Nullable = true,
                        Pattern =
                            @"^([0-9]{1}|(?:0[0-9]|1[0-9]|2[0-3])+):([0-5]?[0-9])(?::([0-5]?[0-9])(?:.(\d{1,9}))?)?$",
                        Example = new OpenApiString("02:00:00")
                    });
            });
    }

    public static IServiceCollection AddAndConfigAuthentication(this IServiceCollection services,
        IConfiguration configuration,
        bool addJwtBearer = true,
        bool addCookie = true)
    {
        var jwtSettings = services.GetOptions<JwtSettings>(nameof(JwtSettings), configuration);
        var authenticationBuilder = services.AddAuthentication(
            options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });
        if (addCookie)
        {
            authenticationBuilder.AddCookie(IdentityConstants.ApplicationScheme);
        }

        if (addJwtBearer)
        {
            authenticationBuilder.AddJwtBearer(
                options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
                    };
                });
        }

        return services;
    }

    private static void AddSwaggerDocs(this SwaggerGenOptions options)
    {
        options.SwaggerDoc(
            "v1",
            new OpenApiInfo
            {
                Version = "v1",
                Title = "API v1",
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });
    }
}