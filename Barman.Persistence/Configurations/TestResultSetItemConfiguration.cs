using Barman.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Barman.Persistence.Configurations;

public class TestResultSetItemConfiguration
    : IEntityTypeConfiguration<TestResultSetItem>
{
    public void Configure(
        EntityTypeBuilder<TestResultSetItem> builder)
    {
        builder.ToTable("TestResultSetItems");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.LOD)
             .HasPrecision(18, 6);

        builder.Property(x => x.LOQ)
            .HasPrecision(18, 6);

        builder.Property(x => x.MinValue)
            .HasPrecision(18, 6);

        builder.Property(x => x.MaxValue)
            .HasPrecision(18, 6);

        // =========================
        // Result Set
        // =========================

        builder.HasOne(x => x.TestResultSet)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.TestResultSetId)
            .OnDelete(DeleteBehavior.Cascade);

        // =========================
        // Result Definition
        // =========================

        builder.HasOne(x => x.TestResultDefinition)
            .WithMany()
            .HasForeignKey(x => x.TestResultDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================
        // Display Order
        // =========================

        builder.Property(x => x.DisplayOrder)
            .IsRequired();

        // =========================
        // Indexes
        // =========================

        builder.HasIndex(x => new
        {
            x.TestResultSetId,
            x.TestResultDefinitionId
        })
        .IsUnique();

        builder.HasIndex(x => new
        {
            x.TestResultSetId,
            x.DisplayOrder
        });
    }
}