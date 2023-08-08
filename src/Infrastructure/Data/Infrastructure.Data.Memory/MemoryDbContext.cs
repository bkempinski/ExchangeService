using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.EntityFramework;
using Shared.EntityFramework.Abstraction;

namespace Infrastructure.Data.Memory;

public class MemoryDbContext : DbContext, IDbContext
{
    public DbSet<Client> Clients { get; set; }
    public DbSet<Trade> Trades { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseInMemoryDatabase("ExchangeService");

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.BuildDomainModel();
}