using ControlHub.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ControlHub.Api.Data;

public class ControlHubDbContext : DbContext
{
    public ControlHubDbContext(
        DbContextOptions<ControlHubDbContext> options)
        : base(options)
    {
    }

    public DbSet<Empresa> Empresas { get; set; } = null!;

    public DbSet<Usuario> Usuarios { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ControlHubDbContext).Assembly);
    }
}