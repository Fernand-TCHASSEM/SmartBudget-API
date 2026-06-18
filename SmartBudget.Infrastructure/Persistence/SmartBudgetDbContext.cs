using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SmartBudget.Domain.Entities;
using SmartBudget.Domain.Interfaces;

namespace SmartBudget.Infrastructure.Persistence;

public class SmartBudgetDbContext(DbContextOptions<SmartBudgetDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<CategoryRule> CategoryRules => Set<CategoryRule>();
    public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
    public DbSet<ImportBatch> ImportBatches => Set<ImportBatch>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Budget> Budgets => Set<Budget>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Use snake_case naming convention for database tables and columns
        optionsBuilder.UseSnakeCaseNamingConvention();
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SmartBudgetDbContext).Assembly);

        // Apply global query filter for soft-deletable entities
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType)) continue;

            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var filter = Expression.Lambda(
                Expression.Equal(
                    Expression.Property(parameter, nameof(ISoftDeletable.DeletedAt)),
                    Expression.Constant(null, typeof(DateTime?))
                ),
                parameter
            );
            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
        }

        base.OnModelCreating(modelBuilder);
    }
}
