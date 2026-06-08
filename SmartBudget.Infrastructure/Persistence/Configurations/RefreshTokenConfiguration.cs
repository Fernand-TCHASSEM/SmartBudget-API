using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBudget.Domain.Entities;

namespace SmartBudget.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).IsRequired().HasMaxLength(36).ValueGeneratedNever();

        builder.Property(r => r.UserId).IsRequired().HasMaxLength(36);

        builder.Property(r => r.Token).IsRequired().HasMaxLength(512);
        builder.Property(r => r.ExpiresAt).IsRequired();
        builder.Property(r => r.IsRevoked).IsRequired();
        builder.Property(r => r.CreatedAt).IsRequired();

        builder.HasOne(r => r.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(u => u.UserId);
        // Added into InitialCreate
        // builder.HasIndex(r => r.Token);
        

        builder.ToTable("refresh_tokens");
    }
}
