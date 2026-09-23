using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Barman.Domain.Entities;

namespace Barman.Persistence.Configurations;

public class CustomFieldDefinitionConfiguration
    : IEntityTypeConfiguration<CustomFieldDefinition>
{
    public void Configure(
        EntityTypeBuilder<CustomFieldDefinition> builder)
    {
        builder.ToTable("CustomFieldDefinitions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.DataType)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SampleCategory)
            .WithMany()
            .HasForeignKey(x => x.SampleCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Matrix)
            .WithMany()
            .HasForeignKey(x => x.MatrixId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.CustomerId,
            x.SampleCategoryId,
            x.MatrixId,
            x.IsActive
        });

        builder.Property(x => x.IsReusable)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(x => x.DisplayOrder);
    }
}