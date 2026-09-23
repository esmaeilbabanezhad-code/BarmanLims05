using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Barman.Domain.Entities;

namespace Barman.Persistence.Configurations;

public class CustomFieldValueConfiguration
    : IEntityTypeConfiguration<CustomFieldValue>
{
    public void Configure(
        EntityTypeBuilder<CustomFieldValue> builder)
    {
        builder.ToTable("CustomFieldValues");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Value)
            .HasMaxLength(4000);

        builder.HasOne(x => x.Sample)
            .WithMany(x => x.CustomFieldValues)
            .HasForeignKey(x => x.SampleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.CustomFieldDefinition)
            .WithMany(x => x.Values)
            .HasForeignKey(x => x.CustomFieldDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.SampleId,
            x.CustomFieldDefinitionId
        })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");
    }
}