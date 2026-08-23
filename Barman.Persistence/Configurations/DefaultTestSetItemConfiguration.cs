using Barman.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Barman.Persistence.Configurations;

public class DefaultTestSetItemConfiguration
    : IEntityTypeConfiguration<DefaultTestSetItem>
{
    public void Configure(
        EntityTypeBuilder<DefaultTestSetItem> builder)
    {
        builder.ToTable("DefaultTestSetItems");

        builder.HasKey(x => x.Id);

        // =========================
        // Item Type
        // =========================

        builder.Property(x => x.IsPanel)
            .IsRequired();

        // =========================
        // Default Test Set
        // =========================

        builder.HasOne(x => x.DefaultTestSet)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.DefaultTestSetId)
            .OnDelete(DeleteBehavior.Cascade);

        // =========================
        // Independent Test
        // =========================

        builder.HasOne(x => x.Test)
            .WithMany()
            .HasForeignKey(x => x.TestId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================
        // Test Panel
        // =========================

        builder.HasOne(x => x.TestPanel)
            .WithMany()
            .HasForeignKey(x => x.TestPanelId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================
        // Sort Order
        // =========================

        builder.Property(x => x.SortOrder)
            .IsRequired();

        // =========================
        // Indexes
        // =========================

        builder.HasIndex(x => new
        {
            x.DefaultTestSetId,
            x.SortOrder
        });

        builder.HasIndex(x => new
        {
            x.DefaultTestSetId,
            x.TestId
        });

        builder.HasIndex(x => new
        {
            x.DefaultTestSetId,
            x.TestPanelId
        });
    }
}