using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ReverseProxy.Extensions;
using Shared.Core.Extensions;
using Shared.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorizationBuilder();
builder.Services.AddSerialization(builder.Configuration);
builder.Services.AddGlobalExceptionHandler();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddCors();
builder.Services.AddAndConfigAuthentication(builder.Configuration);
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddIdentity();

var app = builder.Build();

app.MapAuthenticationControllers();

app.UseSharedInfrastructure(app.Environment);
app.UseCors(
    policyBuilder =>
    {
        policyBuilder.AllowAnyHeader();
        policyBuilder.AllowAnyMethod();
        policyBuilder.AllowAnyOrigin();
    });
app.MapReverseProxy();
app.Run();