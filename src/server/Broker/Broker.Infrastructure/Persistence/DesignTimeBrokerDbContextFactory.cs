using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Shared.Core.Settings;

namespace Broker.Infrastructure.Persistence;

public class DesignTimeBrokerDbContextFactory : IDesignTimeDbContextFactory<BrokerDbContext>
{
    public BrokerDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(Path.GetFullPath("../Broker.Interface/appsettings.json"))
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<BrokerDbContext>();
        var persistenceSettings = configuration.GetSection(nameof(PersistenceSettings)).Get<PersistenceSettings>()!;

        if (persistenceSettings.UsePostgres)
        {
            optionsBuilder.UseNpgsql(
                persistenceSettings.ConnectionStrings.Postgres,
                x =>
                    x.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));
        }
        else if (persistenceSettings.UseMsSql)
        {
            optionsBuilder.UseSqlServer(
                persistenceSettings.ConnectionStrings.MSSQL,
                x =>
                    x.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));
        }
        else if (persistenceSettings.UseSqlite)
        {
            optionsBuilder.UseSqlite(
                persistenceSettings.ConnectionStrings.Sqlite,
                x =>
                    x.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));
        }
        else
        {
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        }

        var persistenceOptions = Options.Create(persistenceSettings);

        return new BrokerDbContext(
            optionsBuilder.Options,
            null,
            persistenceOptions);
    }
}