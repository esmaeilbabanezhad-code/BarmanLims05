using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Barman.Domain.Entities;

namespace Barman.Persistence.Configurations;

public class EmployeeDepartmentConfiguration
    : IEntityTypeConfiguration<EmployeeDepartment>
{
    public void Configure(
        EntityTypeBuilder<EmployeeDepartment> builder)
    {
        builder.ToTable("EmployeeDepartments");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Employee)
            .WithMany(x => x.EmployeeDepartments)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Department)
            .WithMany(x => x.EmployeeDepartments)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.EmployeeId,
            x.DepartmentId
        })
        .IsUnique();
    }
}