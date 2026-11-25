using GameApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GameApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Character> Characters => Set<Character>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Account>(e =>
        {
            e.Property(a => a.Username).IsRequired().HasMaxLength(50);
            e.HasIndex(a => a.Username).IsUnique();
            e.Property(a => a.PasswordHash).IsRequired().HasMaxLength(255);
        });

        modelBuilder.Entity<Character>(e =>
        {
            e.Property(c => c.Name).IsRequired().HasMaxLength(50);
            e.Property(c => c.Class).IsRequired().HasMaxLength(30);

            // 1 Account - n Character, xóa Account thì xóa luôn Character
            e.HasOne(c => c.Account)
                .WithMany(a => a.Characters)
                .HasForeignKey(c => c.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
