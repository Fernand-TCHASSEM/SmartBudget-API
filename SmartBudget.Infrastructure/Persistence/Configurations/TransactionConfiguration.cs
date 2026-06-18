using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBudget.Domain.Entities;
using SmartBudget.Domain.Enums;

namespace SmartBudget.Infrastructure.Persistence.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).IsRequired().HasMaxLength(36).HasColumnType("char(36)").ValueGeneratedNever();

        builder.Property(t => t.UserId).IsRequired().HasMaxLength(36).HasColumnType("char(36)");
        builder.Property(t => t.BankAccountId).IsRequired().HasMaxLength(36).HasColumnType("char(36)");
        builder.Property(t => t.ImportBatchId).IsRequired(false).HasMaxLength(36).HasColumnType("char(36)");
        builder.Property(t => t.CategoryId).IsRequired(false).HasMaxLength(36).HasColumnType("char(36)");

        builder.Property(t => t.RawLabel).IsRequired().HasMaxLength(500);
        builder.Property(t => t.CleanLabel).IsRequired().HasMaxLength(255);
        builder.Property(t => t.Amount)
            .IsRequired()
            .HasColumnType("decimal(12,2)");
        builder.Property(t => t.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasColumnType("enum('DEBIT','CREDIT')");
        builder.Property(t => t.IsCategoryManual).IsRequired().HasDefaultValue(false);
        builder.Property(t => t.Note)
            .IsRequired(false)
            .HasMaxLength(500);
        builder.Property(t => t.IsExcluded).IsRequired().HasDefaultValue(false);
        builder.Property(t => t.Hash)
            .IsRequired()
            .HasMaxLength(64)
            .HasColumnType("char(64)");
        builder.Property(t => t.CreatedAt).IsRequired();
        builder.Property(t => t.UpdatedAt).IsRequired();

        builder.HasOne(t => t.User)
            .WithMany(u => u.Transactions)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.BankAccount)
            .WithMany(ba => ba.Transactions)
            .HasForeignKey(t => t.BankAccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.ImportBatch)
            .WithMany(ib => ib.Transactions)
            .HasForeignKey(t => t.ImportBatchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Category)
            .WithMany(c => c.Transactions)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => new { t.Hash, t.UserId })
            .IsUnique()
            .HasDatabaseName("uq_transactions_hash_user");
        builder.HasIndex(t => t.UserId);
        builder.HasIndex(t => t.BankAccountId);
        builder.HasIndex(t => t.CategoryId);
        builder.HasIndex(t => t.TransactionDate);
        builder.HasIndex(t => t.Type);
        builder.HasIndex(t => t.DeletedAt);

        builder.ToTable("transactions");
    }
}