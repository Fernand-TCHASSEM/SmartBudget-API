using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBudget.Domain.Entities;
using SmartBudget.Domain.Enums;

namespace SmartBudget.Infrastructure.Persistence.Configurations;

public class ImportBatchConfiguration : IEntityTypeConfiguration<ImportBatch>
{
    public void Configure(EntityTypeBuilder<ImportBatch> builder)
    {
        builder.HasKey(ib => ib.Id);
        builder.Property(ib => ib.Id)
            .IsRequired()
            .HasMaxLength(36)
            .ValueGeneratedNever();

        builder.Property(ib => ib.UserId)
            .IsRequired()
            .HasMaxLength(36);
        builder.Property(ib => ib.BankAccountId)
            .IsRequired()
            .HasMaxLength(36);

        builder.Property(ib => ib.FileName).IsRequired().HasMaxLength(255);
        builder.Property(ib => ib.FileType)
            .IsRequired()
            .HasMaxLength(3)
            .HasConversion<string>();
        builder.Property(ib => ib.BlobUrl).IsRequired(false).HasMaxLength(1000);
        builder.Property(ib => ib.Status)
            .HasColumnType("enum('PENDING','PROCESSING','COMPLETED','FAILED')")
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(ImportStatus.PENDING);
        builder.Property(ib => ib.TotalRows).IsRequired().HasDefaultValue(0);
        builder.Property(ib => ib.ImporteCount).IsRequired().HasDefaultValue(0);
        builder.Property(ib => ib.DuplicateCount).IsRequired().HasDefaultValue(0);
        builder.Property(ib => ib.ErrorCount).IsRequired().HasDefaultValue(0);
        builder.Property(ib => ib.ErrorMesage)
            .HasColumnType("text")
            .IsRequired(false);
        builder.Property(ib => ib.ImportedAt).IsRequired();

        builder.HasOne(ib => ib.User)
            .WithMany(u => u.ImportBatches)
            .HasForeignKey(ib => ib.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ib => ib.BankAccount)
            .WithMany(b => b.ImportBatches)
            .HasForeignKey(ib => ib.BankAccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ib => ib.UserId);
        builder.HasIndex(ib => ib.BankAccountId);
        builder.HasIndex(ib => ib.Status);
        builder.HasIndex(ib => ib.DeletedAt);

        builder.ToTable("import_batches");
    }
}
