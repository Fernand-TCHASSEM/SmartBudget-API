using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBudget.Domain.Entities;
using SmartBudget.Domain.Enums;

namespace SmartBudget.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).IsRequired().HasMaxLength(36).HasColumnType("char(36)").ValueGeneratedNever();

        builder.Property(u => u.Email).IsRequired().HasMaxLength(255);
        builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(512);
        builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.LastName).HasMaxLength(100);
        builder.Property(u => u.Currency)
            .IsRequired()
            .HasConversion<string>()
            .HasColumnType("enum('CAD','USD','EUR')")
            .HasDefaultValue(Currency.CAD);
        builder.Property(u => u.MonthStartDay)
            .IsRequired()
            .HasDefaultValue((byte)1)
            .HasColumnType("tinyint unsigned");
        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
        builder.Property(u => u.CreatedAt).IsRequired();
        builder.Property(u => u.UpdatedAt).IsRequired();

        builder
            .HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("uq_users_email");
        builder.HasIndex(u => u.DeletedAt);

        builder.ToTable("users");
    }
}
