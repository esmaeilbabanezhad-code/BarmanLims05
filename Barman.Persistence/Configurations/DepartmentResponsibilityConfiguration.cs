using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Barman.Domain.Entities;

namespace Barman.Persistence.Configurations;

public class DepartmentResponsibilityConfiguration
    : IEntityTypeConfiguration<DepartmentResponsibility>
{
    public void Configure(
        EntityTypeBuilder<DepartmentResponsibility> builder)
    {
        builder.ToTable("DepartmentResponsibilities");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Employee)
            .WithMany(x => x.DepartmentResponsibilities)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Department)
            .WithMany(x => x.Responsibilities)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.EmployeeId,
            x.DepartmentId,
            x.ResponsibilityType
        })
        .IsUnique();
    }
}
