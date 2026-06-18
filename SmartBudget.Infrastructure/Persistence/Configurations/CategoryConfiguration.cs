using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBudget.Domain.Entities;

namespace SmartBudget.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).IsRequired().HasMaxLength(36).HasColumnType("char(36)").ValueGeneratedNever();

        builder.Property(c => c.UserId).IsRequired(false).HasMaxLength(36).HasColumnType("char(36)");

        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Icon).IsRequired().HasMaxLength(10).HasDefaultValue("❓");
        builder.Property(c => c.Color).IsRequired().HasMaxLength(7).HasColumnType("char(7)");
        builder.Property(c => c.IsDefault).IsRequired().HasDefaultValue(false);
        builder.Property(c => c.IsIncome).IsRequired().HasDefaultValue(false);
        builder.Property(c => c.SortOrder).IsRequired().HasDefaultValue(0);
        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.UpdatedAt).IsRequired();

        builder.HasOne(c => c.User)
            .WithMany(u => u.Categories)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.UserId);
        builder.HasIndex(u => u.DeletedAt);

        builder.ToTable("categories");
    }
}
