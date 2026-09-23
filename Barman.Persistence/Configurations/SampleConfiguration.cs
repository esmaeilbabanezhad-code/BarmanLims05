using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Barman.Domain.Entities;

namespace Barman.Persistence.Configurations;

public class SampleConfiguration : IEntityTypeConfiguration<Sample>
{
    public void Configure(EntityTypeBuilder<Sample> builder)
    {
        builder.ToTable("Samples");

        builder.HasKey(x => x.Id);

        // =============================
        // Sample Code
        // =============================

        builder.Property(x => x.SampleCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.SampleCode)
            .IsUnique();

        // =============================
        // Sample Name
        // =============================

        builder.Property(x => x.SampleName)
            .HasMaxLength(300)
            .IsRequired();

        // =============================
        // Sample Category
        // =============================

        builder.HasOne(x => x.SampleCategory)
    .WithMany(x => x.Samples)
    .HasForeignKey(x => x.SampleCategoryId)
    .OnDelete(DeleteBehavior.Restrict);

        // =============================
        // Matrix
        // =============================

        builder.HasOne(x => x.Matrix)
            .WithMany()
            .HasForeignKey(x => x.MatrixId)
            .OnDelete(DeleteBehavior.Restrict);

        // =============================
        // Standard Sample
        // =============================

        builder.HasOne(x => x.StandardSample)
            .WithMany(x => x.Samples)
            .HasForeignKey(x => x.StandardSampleId)
            .OnDelete(DeleteBehavior.Restrict);
        // =============================
        // Other Information
        // =============================



        builder.Property(x => x.Unit)
            .HasMaxLength(50);

        builder.Property(x => x.ContainerType)
            .HasMaxLength(100);

        builder.Property(x => x.CustomerSampleName)
             .HasMaxLength(300);

        builder.Property(x => x.BatchLotNumber)
            .HasMaxLength(100);

        builder.Property(x => x.QuotaNumber)
            .HasMaxLength(100);

        builder.Property(x => x.ShipmentNumber)
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 3);

        // =============================
        // Reception
        // =============================

        builder.HasOne(x => x.Reception)
            .WithMany(x => x.Samples)
            .HasForeignKey(x => x.ReceptionId)
            .OnDelete(DeleteBehavior.Cascade);

        // =============================
        // Test Assignments
        // =============================

        builder.HasMany(x => x.TestAssignments)
            .WithOne(x => x.Sample)
            .HasForeignKey(x => x.SampleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}