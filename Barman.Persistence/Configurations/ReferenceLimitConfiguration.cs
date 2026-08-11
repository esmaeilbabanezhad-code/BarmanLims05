using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Barman.Domain.Entities;

namespace Barman.Persistence.Configurations;

public class ReferenceLimitConfiguration : IEntityTypeConfiguration<ReferenceLimit>
{
    public void Configure(EntityTypeBuilder<ReferenceLimit> builder)
    {
        builder.ToTable("ReferenceLimits");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProductName)
            .HasMaxLength(300);

        builder.Property(x => x.OrganizationName)
            .HasMaxLength(300);

        builder.Property(x => x.Unit)
            .HasMaxLength(50);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.MinValue)
            .HasPrecision(18, 6);

        builder.Property(x => x.MaxValue)
            .HasPrecision(18, 6);

        builder.Property(x => x.WarningLow)
            .HasPrecision(18, 6);

        builder.Property(x => x.WarningHigh)
            .HasPrecision(18, 6);

        builder.HasIndex(x => new
        {
            x.TestId,
            x.MatrixId,
            x.OrganizationName
        });

        builder.HasOne(x => x.Test)
    .WithMany(t => t.ReferenceLimits)
    .HasForeignKey(x => x.TestId)
    .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Matrix)
            .WithMany()
            .HasForeignKey(x => x.MatrixId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}