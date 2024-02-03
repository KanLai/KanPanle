using Microsoft.EntityFrameworkCore;
using WebApplication1.Model;

namespace WebApplication1;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public required DbSet<User> Users { get; init; }
    public required DbSet<Menu> Menus { get; init; }
    public required DbSet<Config> Configs { get; init; }
    public required DbSet<Case> Cases { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Case>()
            .HasMany<Menu>(o => o.menus)
            .WithOne(m => m.@case)
            .HasForeignKey(m => m.belong);

    }
}