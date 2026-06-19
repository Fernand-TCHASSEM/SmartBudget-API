using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBudget.Domain.Entities;

namespace SmartBudget.Infrastructure.Persistence.Configurations;

public class BudgetConfiguration : IEntityTypeConfiguration<Budget>
{
    public void Configure(EntityTypeBuilder<Budget> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id)
            .IsRequired()
            .HasMaxLength(36)
            .ValueGeneratedNever();

        builder.Property(b => b.UserId)
            .IsRequired()
            .HasMaxLength(36);
        builder.Property(b => b.CategoryId)
            .IsRequired()
            .HasMaxLength(36);

        builder.Property(b => b.Year)
            .HasColumnType("smallint")
            .IsRequired();
        builder.Property(b => b.Month)
            .HasColumnType("tinyint unsigned")
            .IsRequired()
            .HasDefaultValue((byte)1);
        builder.Property(t => t.LimitAmount)
            .HasColumnType("decimal(12,2)")
            .IsRequired();
        builder.Property(b => b.CreatedAt).IsRequired();
        builder.Property(b => b.UpdatedAt).IsRequired();

        builder.HasOne(b => b.User)
            .WithMany(u => u.Budgets)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.Category)
            .WithMany(c => c.Budgets)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => new { t.UserId, t.CategoryId, t.Year, t.Month })
            .IsUnique()
            .HasDatabaseName("uq_budgets_user_category_period");
        builder.HasIndex(b => b.UserId);
        builder.HasIndex(b => b.Month);
        builder.HasIndex(b => b.DeletedAt);

        builder.ToTable("budgets");
    }
}
