using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared.EntityFramework;
using Shared.EntityFramework.Abstraction;

namespace Infrastructure.Data.Sqlite;

public class SqliteDbContext : DbContext, IDbContext
{
    private readonly IConfiguration _configuration;

    public DbSet<Client> Clients { get; set; }
    public DbSet<Trade> Trades { get; set; }

    public SqliteDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
        Database.Migrate();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite($"Data Source={Path.Combine(Environment.CurrentDirectory, _configuration.GetConnectionString("SqliteDbName"))}");

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.BuildDomainModel();
}