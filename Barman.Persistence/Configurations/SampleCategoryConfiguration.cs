using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Barman.Domain.Entities;

namespace Barman.Persistence.Configurations;

public class SampleCategoryConfiguration
    : IEntityTypeConfiguration<SampleCategory>
{
    public void Configure(EntityTypeBuilder<SampleCategory> builder)
    {
        builder.ToTable("SampleCategories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.HasIndex(x => x.Name)
            .IsUnique();

        // رابطه SampleCategory → Samples
        // این رابطه در SampleConfiguration به‌صورت کامل تعریف می‌شود.
    }
}