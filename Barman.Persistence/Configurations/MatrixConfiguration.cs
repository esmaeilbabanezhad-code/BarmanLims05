using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Barman.Domain.Entities;

namespace Barman.Persistence.Configurations;

public class MatrixConfiguration : IEntityTypeConfiguration<Matrix>
{
    public void Configure(EntityTypeBuilder<Matrix> builder)
    {
        builder.ToTable("Matrices");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.HasIndex(x => new
        {
            x.SampleCategoryId,
            x.Code
        })
        .IsUnique();

        builder.HasIndex(x => new
        {
            x.SampleCategoryId,
            x.Name
        })
        .IsUnique();

        builder.HasOne(x => x.SampleCategory)
            .WithMany()
            .HasForeignKey(x => x.SampleCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
