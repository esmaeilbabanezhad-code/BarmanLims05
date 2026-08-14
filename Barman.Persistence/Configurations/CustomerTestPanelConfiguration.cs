using Barman.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Barman.Persistence.Configurations;

public class CustomerTestPanelConfiguration
    : IEntityTypeConfiguration<CustomerTestPanel>
{
    public void Configure(
        EntityTypeBuilder<CustomerTestPanel> builder)
    {
        builder.ToTable("CustomerTestPanels");

        builder.HasKey(x => x.Id);

        // =========================
        // Customer
        // =========================

        builder.HasOne(x => x.Customer)
            .WithMany(x => x.TestPanelRules)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        // =========================
        // Sample Category
        // =========================

        builder.HasOne(x => x.SampleCategory)
            .WithMany(x => x.TestPanelRules)
            .HasForeignKey(x => x.SampleCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================
        // Matrix
        // =========================

        builder.HasOne(x => x.Matrix)
            .WithMany(x => x.TestPanelRules)
            .HasForeignKey(x => x.MatrixId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================
        // Test Panel
        // =========================

        builder.HasOne(x => x.TestPanel)
            .WithMany()
            .HasForeignKey(x => x.TestPanelId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================
        // Rule Settings
        // =========================

        builder.Property(x => x.Priority)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        // =========================
        // Indexes
        // =========================

        builder.HasIndex(x => new
        {
            x.CustomerId,
            x.SampleCategoryId,
            x.MatrixId,
            x.Priority
        });
    }
}