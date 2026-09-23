using Barman.Domain.Entities;
using Barman.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Barman.Persistence.Configurations;

public class TestAssignmentConfiguration
    : IEntityTypeConfiguration<TestAssignment>
{
    public void Configure(
        EntityTypeBuilder<TestAssignment> builder)
    {
        builder.ToTable("TestAssignments");

        builder.HasKey(x => x.Id);

        // =============================
        // Workflow Stage
        // =============================

        builder.Property(x => x.WorkflowStage)
            .HasConversion<int>()
            .HasDefaultValue(TestAssignmentWorkflowStage.TechnicalManagerAssignment)
            .IsRequired();

        // =============================
        // Selected Limit Rule
        // =============================

        builder.HasOne(x => x.SelectedLimitRule)
            .WithMany()
            .HasForeignKey(x => x.SelectedLimitRuleId)
            .OnDelete(DeleteBehavior.Restrict);

        // =============================
        // Test Panel
        // =============================

        builder.HasOne(x => x.TestPanel)
            .WithMany()
            .HasForeignKey(x => x.TestPanelId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.HasOne(x => x.TestResultSet)
            .WithMany()
            .HasForeignKey(x => x.TestResultSetId)
            .OnDelete(DeleteBehavior.Restrict);

        // =============================
        // Technical Manager
        // =============================

        builder.HasOne(x => x.TechnicalManager)
            .WithMany()
            .HasForeignKey(x => x.TechnicalManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        // =============================
        // Technical Manager Scope
        // =============================

        builder.HasOne(x => x.TechnicalManagerScope)
            .WithMany()
            .HasForeignKey(x => x.TechnicalManagerScopeId)
            .OnDelete(DeleteBehavior.Restrict);

        // =============================
        // Section Head
        // =============================

    }
}
