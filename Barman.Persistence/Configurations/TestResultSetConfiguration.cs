using Barman.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Barman.Persistence.Configurations;

public class TestResultSetConfiguration
    : IEntityTypeConfiguration<TestResultSet>
{
    public void Configure(
        EntityTypeBuilder<TestResultSet> builder)
    {
        builder.ToTable("TestResultSets");

        builder.HasKey(x => x.Id);

        // =========================
        // Identity
        // =========================

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        // =========================
        // Test
        // =========================

        builder.HasOne(x => x.Test)
            .WithMany(x => x.ResultSets)
            .HasForeignKey(x => x.TestId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================
        // Customer
        // =========================

        builder.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================
        // Sample Category
        // =========================

        builder.HasOne(x => x.SampleCategory)
            .WithMany()
            .HasForeignKey(x => x.SampleCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================
        // Matrix
        // =========================

        builder.HasOne(x => x.Matrix)
            .WithMany()
            .HasForeignKey(x => x.MatrixId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================
        // Standard Sample
        // =========================

        builder.HasOne(x => x.StandardSample)
            .WithMany()
            .HasForeignKey(x => x.StandardSampleId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================
        // Priority
        // =========================

        builder.Property(x => x.Priority)
            .IsRequired();

        // =========================
        // Items
        // =========================

        builder.HasMany(x => x.Items)
            .WithOne(x => x.TestResultSet)
            .HasForeignKey(x => x.TestResultSetId)
            .OnDelete(DeleteBehavior.Cascade);

        // =========================
        // Indexes
        // =========================

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.HasIndex(x => new
        {
            x.TestId,
            x.CustomerId,
            x.SampleCategoryId,
            x.MatrixId,
            x.StandardSampleId,
            x.Priority
        });
    }
}