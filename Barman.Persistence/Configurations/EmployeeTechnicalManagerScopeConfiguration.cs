using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Barman.Domain.Entities;

namespace Barman.Persistence.Configurations;

public class EmployeeTechnicalManagerScopeConfiguration
    : IEntityTypeConfiguration<EmployeeTechnicalManagerScope>
{
    public void Configure(
        EntityTypeBuilder<EmployeeTechnicalManagerScope> builder)
    {
        builder.ToTable("EmployeeTechnicalManagerScopes");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Employee)
            .WithMany(x => x.TechnicalManagerScopes)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.TechnicalManagerScope)
            .WithMany(x => x.EmployeeScopes)
            .HasForeignKey(x => x.TechnicalManagerScopeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new
        {
            x.EmployeeId,
            x.TechnicalManagerScopeId
        })
        .IsUnique();
    }
}