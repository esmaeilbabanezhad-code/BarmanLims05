using Barman.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Barman.Persistence.Configurations;

public class TechnicalManagerRoutingRuleConfiguration
    : IEntityTypeConfiguration<TechnicalManagerRoutingRule>
{
    public void Configure(
        EntityTypeBuilder<TechnicalManagerRoutingRule> builder)
    {
        builder.ToTable("TechnicalManagerRoutingRules");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Priority)
            .IsRequired()
            .HasDefaultValue(100);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SampleCategory)
            .WithMany()
            .HasForeignKey(x => x.SampleCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Matrix)
            .WithMany()
            .HasForeignKey(x => x.MatrixId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TestPanel)
            .WithMany()
            .HasForeignKey(x => x.TestPanelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Test)
            .WithMany()
            .HasForeignKey(x => x.TestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Department)
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TechnicalManagerScope)
            .WithMany()
            .HasForeignKey(x => x.TechnicalManagerScopeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.HasIndex(x => new
        {
            x.Priority,
            x.IsActive
        });
    }
}