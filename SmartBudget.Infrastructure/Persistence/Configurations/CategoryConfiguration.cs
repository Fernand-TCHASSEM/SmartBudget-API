using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBudget.Domain.Entities;

namespace SmartBudget.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .IsRequired()
            .HasMaxLength(36)
            .IsFixedLength()
            .ValueGeneratedNever();

        builder.Property(c => c.UserId)
            .IsRequired(false)
            .HasMaxLength(36)
            .IsFixedLength();

        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Icon).IsRequired().HasMaxLength(10).HasDefaultValue("❓");
        builder.Property(c => c.Color).HasColumnType("char(7)").IsRequired().HasMaxLength(7).IsFixedLength();
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
        builder.HasIndex(c => c.DeletedAt);

        builder.ToTable("categories");
    }
}
