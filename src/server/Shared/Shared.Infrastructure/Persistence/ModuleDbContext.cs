using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Core.Domain;
using Shared.Core.Interfaces;
using Shared.Core.Settings;
using Shared.Infrastructure.Extensions;

namespace Shared.Infrastructure.Persistence;

public abstract class ModuleDbContext : DbContext, IModuleDbContext
{
    private readonly IMediator _mediator;
    private readonly PersistenceSettings _persistenceOptions;

    protected ModuleDbContext(DbContextOptions options,
        IMediator mediator,
        IOptions<PersistenceSettings> persistenceOptions)
        : base(options)
    {
        _mediator = mediator;
        _persistenceOptions = persistenceOptions.Value;
    }

    protected abstract string Schema { get; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await this.SaveChangeWithPublishEventsAsync(_mediator, cancellationToken);
    }

    public override int SaveChanges()
    {
        return this.SaveChangeWithPublishEvents(_mediator);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (!string.IsNullOrWhiteSpace(Schema))
        {
            modelBuilder.HasDefaultSchema(Schema);
        }

        modelBuilder.Ignore<Event>();
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        modelBuilder.ApplyModuleConfiguration(_persistenceOptions);
    }


    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        return SaveChanges();
    }
}