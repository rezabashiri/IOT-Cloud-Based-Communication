using System.Text.Json.Serialization;
using Broker.Application.Extensions;
using Broker.Interface.Extensions;
using Shared.Core.Extensions;
using Shared.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization();
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddGrpc().AddJsonTranscoding();
builder.Services.AddGrpcSwagger();
builder.Services.AddSerialization(builder.Configuration);
builder.Services.AddSharedInfrastructure(builder.Configuration);
builder.Services.AddSharedApplication(builder.Configuration);
builder.Services.AddBrokerApplication(builder.Configuration);

builder.Services.AddAndConfigAuthentication(builder.Configuration, addCookie: false);

var app = builder.Build();

app.UseSharedInfrastructure(app.Environment, "internal/swagger");
app.UseApplication();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UsegRpcServices();

app.Run();