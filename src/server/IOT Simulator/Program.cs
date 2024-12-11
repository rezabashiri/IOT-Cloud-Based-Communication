// See https://aka.ms/new-console-template for more information


using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PLC_Simulator;
using Shared.Core.Extensions;
using Shared.Infrastructure.Extensions;

var configuration = new ConfigurationBuilder()
    .AddJsonFile(Path.GetFullPath("./appsettings.json"))
    .Build();


var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(
        collection =>
        {
            collection.AddSingleton<IConfiguration>(_ => configuration);
            collection.AddMqttBroker(configuration);
            collection.AddSerialization(configuration);
        });
var builtHost = host.Build();

await Controller.Run(builtHost.Services);
await builtHost.RunAsync();