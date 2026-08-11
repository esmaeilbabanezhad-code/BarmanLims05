using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Barman.Domain.Entities;

namespace Barman.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.DisplayName)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(x => x.LegalName)
            .HasMaxLength(500);

        builder.Property(x => x.NationalId)
            .HasMaxLength(50);

        builder.Property(x => x.EconomicCode)
            .HasMaxLength(50);

        builder.Property(x => x.RegistrationNo)
            .HasMaxLength(50);

        builder.Property(x => x.Province)
            .HasMaxLength(100);

        builder.Property(x => x.City)
            .HasMaxLength(100);

        builder.Property(x => x.Address)
            .HasMaxLength(1000);

        builder.Property(x => x.PostalCode)
            .HasMaxLength(30);

        builder.Property(x => x.Phone)
            .HasMaxLength(50);

        builder.Property(x => x.Mobile)
            .HasMaxLength(50);

        builder.Property(x => x.Email)
            .HasMaxLength(300);

        builder.Property(x => x.Website)
            .HasMaxLength(300);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.HasIndex(x => x.Code)
            .IsUnique();
    }
}