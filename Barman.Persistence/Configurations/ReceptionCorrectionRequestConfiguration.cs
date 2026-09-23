using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Barman.Domain.Entities;

namespace Barman.Persistence.Configurations;

public class ReceptionCorrectionRequestConfiguration
    : IEntityTypeConfiguration<ReceptionCorrectionRequest>
{
    public void Configure(
        EntityTypeBuilder<ReceptionCorrectionRequest> builder)
    {
        builder.ToTable("ReceptionCorrectionRequests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Reason)
            .HasMaxLength(4000)
            .IsRequired();

        builder.HasOne(x => x.Reception)
            .WithMany()
            .HasForeignKey(x => x.ReceptionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Sample)
            .WithMany()
            .HasForeignKey(x => x.SampleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.RequestedByEmployee)
            .WithMany()
            .HasForeignKey(x => x.RequestedByEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ResolvedByEmployee)
            .WithMany()
            .HasForeignKey(x => x.ResolvedByEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.ReceptionId,
            x.Status
        });
    }
}