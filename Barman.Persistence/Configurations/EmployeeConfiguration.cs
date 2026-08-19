using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Barman.Domain.Entities;

namespace Barman.Persistence.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PersonnelCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.FullName)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(x => x.NationalCode)
            .HasMaxLength(20);

        builder.Property(x => x.Mobile)
            .HasMaxLength(30);

        builder.Property(x => x.Email)
            .HasMaxLength(200);

        builder.HasIndex(x => x.PersonnelCode)
            .IsUnique();
    }
}