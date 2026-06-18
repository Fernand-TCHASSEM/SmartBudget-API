using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBudget.Domain.Entities;

namespace SmartBudget.Infrastructure.Persistence.Configurations;

public class CategoryRuleConfiguration : IEntityTypeConfiguration<CategoryRule>
{
    public void Configure(EntityTypeBuilder<CategoryRule> builder)
    {
        builder.HasKey(cr => cr.Id);
        builder.Property(cr => cr.Id)
            .IsRequired()
            .HasMaxLength(36)
            .IsFixedLength()
            .ValueGeneratedNever();

        builder.Property(cr => cr.UserId)
            .IsRequired(false)
            .HasMaxLength(36)
            .IsFixedLength();
        builder.Property(cr => cr.CategoryId)
            .IsRequired()
            .HasMaxLength(36)
            .IsFixedLength();

        builder.Property(cr => cr.Name).IsRequired(false).HasMaxLength(150);
        builder.Property(cr => cr.Keyword).IsRequired().HasMaxLength(255);
        builder.Property(cr => cr.IsRegex).IsRequired().HasDefaultValue(false);
        builder.Property(cr => cr.Priority).IsRequired().HasDefaultValue(100);
        builder.Property(cr => cr.CreatedAt).IsRequired();

        builder.HasOne(cr => cr.User)
            .WithMany(u => u.CategoryRules)
            .HasForeignKey(cr => cr.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cr => cr.Category)
            .WithMany(c => c.CategoryRules)
            .HasForeignKey(cr => cr.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(cr => cr.UserId);
        builder.HasIndex(cr => cr.CategoryId);
        builder.HasIndex(cr => cr.Priority);
        builder.HasIndex(cr => cr.DeletedAt);

        builder.ToTable("category_rules");
    }
}
