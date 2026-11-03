using MetallurgyApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MetallurgyApp.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ProductionArea> ProductionAreas => Set<ProductionArea>();
    public DbSet<Equipment> Equipments => Set<Equipment>();

    public DbSet<User> Users => Set<User>();

    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<ProductionArea>()
          .HasMany(a => a.Equipments)
          .WithOne(e => e.ProductionArea)
          .HasForeignKey(e => e.ProductionAreaId)
          .OnDelete(DeleteBehavior.Cascade);

        mb.Entity<User>()
            .HasIndex(u => u.UserName)
            .IsUnique();

        mb.Entity<ChatMessage>(e =>
        {
            e.HasKey(m => m.Id);
            e.Property(m => m.Id).ValueGeneratedNever();
            e.Property(m => m.UserName).IsRequired().HasMaxLength(100);
            e.Property(m => m.Text).IsRequired().HasMaxLength(2000);
            e.HasIndex(m => m.SentAt);
        });
    }
}