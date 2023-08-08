using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Shared.EntityFramework;

public static class EntityFrameworkExtensions
{
    public static ModelBuilder BuildDomainModel(this ModelBuilder modelBuilder)
    {
        // Client
        modelBuilder
            .Entity<Client>()
            .Property(t => t.Id)
            .ValueGeneratedOnAdd();

        modelBuilder
            .Entity<Client>()
            .HasIndex(c => c.IpAddress)
            .IsUnique();

        // Trade
        modelBuilder
            .Entity<Trade>()
            .Property(t => t.Id)
            .ValueGeneratedOnAdd();

        modelBuilder
            .Entity<Trade>()
            .HasOne(t => t.Client)
            .WithMany(c => c.Trades)
            .HasForeignKey(t => t.ClientId);

        return modelBuilder;
    }
}