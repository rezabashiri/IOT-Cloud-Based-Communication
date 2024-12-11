using Broker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Interfaces;

namespace Broker.Domain.Abstractions;

public interface IBrokerDbContext : IDbContext
{
    DbSet<Machine> Machines { get; set; }
}