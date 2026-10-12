using MetallurgyApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MetallurgyApp.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ProductionArea> ProductionAreas => Set<ProductionArea>();
    public DbSet<Equipment> Equipments => Set<Equipment>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<ProductionArea>()
          .HasMany(a => a.Equipments)
          .WithOne(e => e.ProductionArea)
          .HasForeignKey(e => e.ProductionAreaId)
          .OnDelete(DeleteBehavior.Cascade);
    }
}