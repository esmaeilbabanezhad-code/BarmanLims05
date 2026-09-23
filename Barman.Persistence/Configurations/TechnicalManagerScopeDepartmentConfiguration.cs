using Barman.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Barman.Persistence.Configurations;

public class TechnicalManagerScopeDepartmentConfiguration
    : IEntityTypeConfiguration<TechnicalManagerScopeDepartment>
{
    public void Configure(
        EntityTypeBuilder<TechnicalManagerScopeDepartment> builder)
    {
        builder.ToTable("TechnicalManagerScopeDepartments");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.TechnicalManagerScope)
            .WithMany()
            .HasForeignKey(x => x.TechnicalManagerScopeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Department)
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Priority)
            .IsRequired()
            .HasDefaultValue(100);

        builder.Property(x => x.IsPrimary)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(x => new
        {
            x.TechnicalManagerScopeId,
            x.DepartmentId
        })
        .IsUnique();
    }
}
