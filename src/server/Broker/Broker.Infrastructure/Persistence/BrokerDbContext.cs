using Broker.Domain.Abstractions;
using Broker.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Core.Settings;
using Shared.Infrastructure.Persistence;

namespace Broker.Infrastructure.Persistence;

public class BrokerDbContext : ModuleDbContext, IBrokerDbContext
{
    public BrokerDbContext(DbContextOptions options,
        IMediator mediator,
        IOptions<PersistenceSettings> persistenceOptions) : base(
        options,
        mediator,
        persistenceOptions)
    {
    }

    protected override string Schema => "Broker";

    public DbSet<Machine> Machines { get; set; }
}