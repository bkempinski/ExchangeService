using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared.EntityFramework;
using Shared.EntityFramework.Abstraction;

namespace Infrastructure.Data.SqlServer;

public class SqlServerDbContext : DbContext, IDbContext
{
    private readonly IConfiguration _configuration;

    public DbSet<Client> Clients { get; set; }
    public DbSet<Trade> Trades { get; set; }

    public SqlServerDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
        Database.Migrate();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlServer(_configuration.GetConnectionString("SqlServerConnectionString"));

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.BuildDomainModel();
}