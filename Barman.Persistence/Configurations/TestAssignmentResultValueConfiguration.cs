using Barman.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Barman.Persistence.Configurations;

public class TestAssignmentResultValueConfiguration
    : IEntityTypeConfiguration<TestAssignmentResultValue>
{
    public void Configure(
        EntityTypeBuilder<TestAssignmentResultValue> builder)
    {
        builder.ToTable("TestAssignmentResultValues");

        builder.HasKey(x => x.Id);

        // =============================
        // Test Assignment
        // =============================

        builder.HasOne(x => x.TestAssignment)
            .WithMany(x => x.ResultValues)
            .HasForeignKey(x => x.TestAssignmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // =============================
        // Result Set Item
        // =============================

        builder.HasOne(x => x.TestResultSetItem)
            .WithMany()
            .HasForeignKey(x => x.TestResultSetItemId)
            .OnDelete(DeleteBehavior.Restrict);

        // =============================
        // Value
        // =============================

        builder.Property(x => x.Value)
            .HasMaxLength(2000);

        // =============================
        // Comment
        // =============================

        builder.Property(x => x.Comment)
            .HasMaxLength(4000);

        // =============================
        // Prevent duplicate value
        // for same Assignment + ResultSetItem
        // =============================

        builder.HasIndex(x => new
        {
            x.TestAssignmentId,
            x.TestResultSetItemId
        })
        .IsUnique();
    }
}