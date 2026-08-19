using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Barman.Domain.Entities;

namespace Barman.Persistence.Configurations;

public class TechnicalManagerSectionHeadConfiguration
    : IEntityTypeConfiguration<TechnicalManagerSectionHead>
{
    public void Configure(
        EntityTypeBuilder<TechnicalManagerSectionHead> builder)
    {
        builder.ToTable("TechnicalManagerSectionHeads");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.TechnicalManager)
            .WithMany(x => x.TechnicalManagerSectionHeads)
            .HasForeignKey(x => x.TechnicalManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SectionHead)
            .WithMany(x => x.SectionHeadTechnicalManagers)
            .HasForeignKey(x => x.SectionHeadId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.TechnicalManagerId,
            x.SectionHeadId
        })
        .IsUnique();
    }
}
