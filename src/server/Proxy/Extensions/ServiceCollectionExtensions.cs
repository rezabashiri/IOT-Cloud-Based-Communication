using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ReverseProxy.Persistence;

namespace ReverseProxy.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services.AddDbContext<AuthenticationDbContext>(options => options.UseInMemoryDatabase("BrokerDb"));
        services.AddIdentityCore<IdentityUser>(
                options =>
                {
                    options.Tokens.AuthenticatorTokenProvider = null;

                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 3;
                    options.Password.RequiredUniqueChars = 0;

                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Lockout time (in minutes)
                    options.Lockout.MaxFailedAccessAttempts = 5; // Max number of failed attempts before lockout
                    options.Lockout.AllowedForNewUsers = true; // Enable lockout for new users

                    // Configure SignIn Settings
                    options.SignIn.RequireConfirmedAccount = false;

                    // Optional: Disable 2FA entirely if you want
                    options.Tokens.ProviderMap.Clear();
                })
            .AddEntityFrameworkStores<AuthenticationDbContext>();

        return services;
    }
}