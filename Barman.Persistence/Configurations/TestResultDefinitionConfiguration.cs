using Barman.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Barman.Persistence.Configurations;

public class TestResultDefinitionConfiguration
    : IEntityTypeConfiguration<TestResultDefinition>
{
    public void Configure(
        EntityTypeBuilder<TestResultDefinition> builder)
    {
        builder.ToTable("TestResultDefinitions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(x => x.Unit)
            .HasMaxLength(50);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.DisplayOrder)
            .IsRequired();

        builder.Property(x => x.IsRequired)
            .IsRequired();

        builder.HasOne(x => x.Test)
            .WithMany(x => x.ResultDefinitions)
            .HasForeignKey(x => x.TestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new
        {
            x.TestId,
            x.Code
        })
        .IsUnique();

        builder.HasIndex(x => new
        {
            x.TestId,
            x.DisplayOrder
        });
    }
}