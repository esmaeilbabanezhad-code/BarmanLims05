using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Barman.Domain.Entities;

namespace Barman.Persistence.Configurations;

public class EmployeeRoleConfiguration
    : IEntityTypeConfiguration<EmployeeRole>
{
    public void Configure(
        EntityTypeBuilder<EmployeeRole> builder)
    {
        builder.ToTable("EmployeeRoles");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Employee)
            .WithMany(x => x.EmployeeRoles)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Role)
            .WithMany(x => x.EmployeeRoles)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.EmployeeId,
            x.RoleId
        })
        .IsUnique();
    }
}