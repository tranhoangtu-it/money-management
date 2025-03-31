using Microsoft.EntityFrameworkCore;
using MoneyManagement.API.Models;

namespace MoneyManagement.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Jar> Jars { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.SourceJar)
            .WithMany()
            .HasForeignKey(t => t.SourceJarId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.DestinationJar)
            .WithMany()
            .HasForeignKey(t => t.DestinationJarId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure decimal precision
        modelBuilder.Entity<Jar>()
            .Property(j => j.CurrentBalance)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Jar>()
            .Property(j => j.Percentage)
            .HasPrecision(5, 2);

        modelBuilder.Entity<Transaction>()
            .Property(t => t.Amount)
            .HasPrecision(18, 2);

        // Seed initial jars
        modelBuilder.Entity<Jar>().HasData(
            new Jar { Id = 1, Name = "Necessities", Percentage = 50, Description = "Essential expenses like housing, utilities, groceries", CurrentBalance = 0 },
            new Jar { Id = 2, Name = "Financial Freedom", Percentage = 10, Description = "Long-term investments and wealth building", CurrentBalance = 0 },
            new Jar { Id = 3, Name = "Education", Percentage = 10, Description = "Personal development and learning", CurrentBalance = 0 },
            new Jar { Id = 4, Name = "Long-term Savings", Percentage = 10, Description = "Emergency fund and future goals", CurrentBalance = 0 },
            new Jar { Id = 5, Name = "Play", Percentage = 10, Description = "Entertainment and fun activities", CurrentBalance = 0 },
            new Jar { Id = 6, Name = "Give", Percentage = 10, Description = "Charitable donations and helping others", CurrentBalance = 0 }
        );
    }
} 