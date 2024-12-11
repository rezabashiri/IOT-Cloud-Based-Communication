using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Contracts;
using Shared.Core.Interfaces;

namespace Shared.Infrastructure.Extensions;

public static class ModuleDbContextExtensions
{
    public static async Task<int> SaveChangeWithPublishEventsAsync<TModuleDbContext>(this TModuleDbContext context,
        IMediator mediator,
        CancellationToken cancellationToken = new())
        where TModuleDbContext : DbContext, IModuleDbContext
    {
        var domainEntities = context.ChangeTracker
            .Entries<IBaseEntity>()
            .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEntities.ToList()
            .ForEach(entity => entity.Entity.ClearDomainEvents());

        var tasks = domainEvents
            .Select(async domainEvent => { await mediator.Publish(domainEvent, cancellationToken); });
        await Task.WhenAll(tasks);

        return await context.SaveChangesAsync(true, cancellationToken);
    }

    public static int SaveChangeWithPublishEvents<TModuleDbContext>(
        this TModuleDbContext context,
        IMediator mediator
    )
        where TModuleDbContext : DbContext, IModuleDbContext
    {
        return SaveChangeWithPublishEventsAsync(context, mediator)
            .GetAwaiter()
            .GetResult();
    }
}