using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Barman.Domain.Entities;

namespace Barman.Persistence.Configurations;

public class NumberSequenceConfiguration : IEntityTypeConfiguration<NumberSequence>
{
    public void Configure(EntityTypeBuilder<NumberSequence> builder)
    {
        builder.ToTable("NumberSequences");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EntityName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Prefix)
            .HasMaxLength(20);

        builder.Property(x => x.Separator)
            .HasMaxLength(5);

        builder.HasIndex(x => x.EntityName)
            .IsUnique();
    }
}