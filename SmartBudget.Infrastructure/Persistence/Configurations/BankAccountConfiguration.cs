using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBudget.Domain.Entities;
using SmartBudget.Domain.Enums;

namespace SmartBudget.Infrastructure.Persistence.Configurations;

public class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
{
    public void Configure(EntityTypeBuilder<BankAccount> builder)
    {
        builder.HasKey(ba => ba.Id);
        builder.Property(ba => ba.Id)
            .IsRequired()
            .HasMaxLength(36)
            .IsFixedLength()
            .ValueGeneratedNever();

        builder.Property(ba => ba.UserId)
            .IsRequired()
            .HasMaxLength(36)
            .IsFixedLength();

        builder.Property(ba => ba.Name).IsRequired().HasMaxLength(150);
        builder.Property(ba => ba.BankName).IsRequired().HasMaxLength(150);
        builder.Property(ba => ba.AccountType)
            .HasColumnType("enum('CHEQUING','SAVINGS','CREDIT')")
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(AccountType.CHEQUING);
        builder.Property(ba => ba.Currency)
            .HasColumnType("enum('CAD','USD','EUR')")
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(Currency.CAD);
        builder.Property(ba => ba.CreatedAt).IsRequired();
        builder.Property(ba => ba.UpdatedAt).IsRequired();

        builder.HasOne(ba => ba.User)
            .WithMany(u => u.BankAccounts)
            .HasForeignKey(ba => ba.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(bc => bc.UserId);
        builder.HasIndex(bc => bc.DeletedAt);

        builder.ToTable("bank_accounts");
    }
}