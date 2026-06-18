using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBudget.Domain.Entities;

namespace SmartBudget.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    private static readonly int[] TokenIndexPrefixLength = [255];

    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .IsRequired()
            .HasMaxLength(36)
            .IsFixedLength();

        builder.Property(r => r.UserId)
            .IsRequired()
            .HasMaxLength(36)
            .IsFixedLength();

        builder.Property(r => r.Token).IsRequired().HasMaxLength(512);
        builder.Property(r => r.ExpiresAt).IsRequired();
        builder.Property(u => u.IsRevoked)
            .IsRequired()
            .HasDefaultValue(false);
        builder.Property(r => r.CreatedAt).IsRequired();

        builder.HasOne(r => r.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(u => u.UserId);
        builder.HasIndex(r => r.Token)
            .HasDatabaseName("ix_refresh_tokens_token")
            .HasAnnotation("MySql:IndexPrefixLength", TokenIndexPrefixLength);

        builder.ToTable("refresh_tokens");
    }
}
