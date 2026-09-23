using Barman.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Barman.Persistence.Configurations;

public class TestLimitRuleConfiguration
    : IEntityTypeConfiguration<TestLimitRule>
{
    public void Configure(
        EntityTypeBuilder<TestLimitRule> builder)
    {
        builder.ToTable("TestLimitRules");

        builder.HasKey(x => x.Id);

        // =========================
        // Test
        // =========================

        builder.HasOne(x => x.Test)
            .WithMany()
            .HasForeignKey(x => x.TestId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================
        // Matrix
        // =========================

        builder.HasOne(x => x.Matrix)
            .WithMany()
            .HasForeignKey(x => x.MatrixId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================
        // Sample Category
        // =========================

        builder.HasOne(x => x.SampleCategory)
            .WithMany()
            .HasForeignKey(x => x.SampleCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================
        // Customer
        // =========================

        builder.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================
        // Limit Reference
        // =========================

        builder.HasOne(x => x.LimitReference)
                .WithMany(x => x.TestLimitRules)
                .HasForeignKey(x => x.LimitReferenceId)
                .OnDelete(DeleteBehavior.Restrict);

        // =========================
        // Values
        // =========================

        builder.Property(x => x.AllowedValues)
            .HasMaxLength(2000);

        builder.Property(x => x.Unit)
            .HasMaxLength(50);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        // =========================
        // Index
        // =========================

        builder.HasIndex(x => new
        {
            x.TestId,
            x.CustomerId,
            x.MatrixId,
            x.SampleCategoryId,
            x.IsActive,
            x.ValidFrom,
            x.ValidTo,
            x.Priority
        });
    }
}
