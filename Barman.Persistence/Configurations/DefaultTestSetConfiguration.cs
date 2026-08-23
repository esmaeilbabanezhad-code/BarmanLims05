using Barman.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Barman.Persistence.Configurations;

public class DefaultTestSetConfiguration
    : IEntityTypeConfiguration<DefaultTestSet>
{
    public void Configure(
        EntityTypeBuilder<DefaultTestSet> builder)
    {
        builder.ToTable("DefaultTestSets");

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
        // Settings
        // =========================

        builder.Property(x => x.Priority)
            .IsRequired();

        // =========================
        // Items
        // =========================

        builder.HasMany(x => x.Items)
            .WithOne(x => x.DefaultTestSet)
            .HasForeignKey(x => x.DefaultTestSetId)
            .OnDelete(DeleteBehavior.Cascade);

        // =========================
        // Indexes
        // =========================

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.HasIndex(x => new
        {
            x.CustomerId,
            x.SampleCategoryId,
            x.MatrixId,
            x.Priority
        });
    }
}