using Barman.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Barman.Persistence.Configurations;

public class TestLimitChangeRequestConfiguration
    : IEntityTypeConfiguration<TestLimitChangeRequest>
{
    public void Configure(
        EntityTypeBuilder<TestLimitChangeRequest> builder)
    {
        builder.ToTable("TestLimitChangeRequests");

        builder.HasKey(x => x.Id);

        // -------------------------
        // Reason
        // -------------------------

        builder.Property(x => x.Reason)
            .HasMaxLength(2000);

        // -------------------------
        // ApprovalComment
        // -------------------------

        builder.Property(x => x.ApprovalComment)
            .HasMaxLength(2000);

        // -------------------------
        // Decimal precision
        // -------------------------

        builder.Property(x => x.CurrentLOD)
            .HasPrecision(18, 6);

        builder.Property(x => x.RequestedLOD)
            .HasPrecision(18, 6);

        builder.Property(x => x.CurrentLOQ)
            .HasPrecision(18, 6);

        builder.Property(x => x.RequestedLOQ)
            .HasPrecision(18, 6);

        builder.Property(x => x.CurrentMinValue)
            .HasPrecision(18, 6);

        builder.Property(x => x.RequestedMinValue)
            .HasPrecision(18, 6);

        builder.Property(x => x.CurrentMaxValue)
            .HasPrecision(18, 6);

        builder.Property(x => x.RequestedMaxValue)
            .HasPrecision(18, 6);

        builder.Property(x => x.CurrentWarningLow)
            .HasPrecision(18, 6);

        builder.Property(x => x.RequestedWarningLow)
            .HasPrecision(18, 6);

        builder.Property(x => x.CurrentWarningHigh)
            .HasPrecision(18, 6);

        builder.Property(x => x.RequestedWarningHigh)
            .HasPrecision(18, 6);

        // -------------------------
        // Test
        // -------------------------

        builder.HasOne(x => x.Test)
            .WithMany()
            .HasForeignKey(x => x.TestId)
            .OnDelete(DeleteBehavior.Restrict);

        // -------------------------
        // ReferenceLimit
        // -------------------------

        builder.HasOne(x => x.ReferenceLimit)
            .WithMany()
            .HasForeignKey(x => x.ReferenceLimitId)
            .OnDelete(DeleteBehavior.Restrict);

        // -------------------------
        // Requested By
        // -------------------------

        builder.HasOne(x => x.RequestedByEmployee)
            .WithMany()
            .HasForeignKey(x => x.RequestedByEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // -------------------------
        // Approved By
        // -------------------------

        builder.HasOne(x => x.ApprovedByEmployee)
            .WithMany()
            .HasForeignKey(x => x.ApprovedByEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // -------------------------
        // Technical Manager Approved By
        // -------------------------

        builder.HasOne(x => x.TechnicalManagerApprovedByEmployee)
            .WithMany()
            .HasForeignKey(x => x.TechnicalManagerApprovedByEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.TechnicalManagerApprovalComment)
            .HasMaxLength(2000);
        // -------------------------
        // Indexes
        // -------------------------

        builder.HasIndex(x => new
        {
            x.TestId,
            x.Status
        });

        builder.HasIndex(x => x.RequestedByEmployeeId);

        builder.HasIndex(x => x.ApprovedByEmployeeId);
    }
}