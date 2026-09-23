using Barman.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Barman.Persistence.Configurations;

public class TestResultReviewConfiguration
    : IEntityTypeConfiguration<TestResultReview>
{
    public void Configure(
        EntityTypeBuilder<TestResultReview> builder)
    {
        builder.ToTable("TestResultReviews");

        builder.HasKey(x => x.Id);

        // =============================
        // Test Assignment
        // =============================

        builder.HasOne(x => x.TestAssignment)
            .WithMany()
            .HasForeignKey(x => x.TestAssignmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // =============================
        // Reviewer Employee
        // =============================

        builder.HasOne(x => x.ReviewerEmployee)
            .WithMany()
            .HasForeignKey(x => x.ReviewerEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // =============================
        // Reason
        // =============================

        builder.Property(x => x.Reason)
            .HasMaxLength(2000);

        // =============================
        // Review Level / Decision
        // =============================

        builder.Property(x => x.ReviewLevel)
            .IsRequired();

        builder.Property(x => x.Decision)
            .IsRequired();

        // =============================
        // Index
        // =============================

        builder.HasIndex(x => new
        {
            x.TestAssignmentId,
            x.ReviewLevel,
            x.CreatedAt
        });
    }
}
