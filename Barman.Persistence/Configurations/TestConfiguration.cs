
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Barman.Domain.Entities;

namespace Barman.Persistence.Configurations;

public class TestConfiguration : IEntityTypeConfiguration<Test>
{
    public void Configure(EntityTypeBuilder<Test> builder)
    {
        builder.ToTable("Tests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(x => x.EnglishName)
            .HasMaxLength(300);

        builder.Property(x => x.Unit)
            .HasMaxLength(50);

        builder.Property(x => x.Method)
            .HasMaxLength(300);

        builder.Property(x => x.InstrumentName)
            .HasMaxLength(300);

        builder.Property(x => x.DefaultResult)
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.LOD)
            .HasPrecision(18, 6);

        builder.Property(x => x.LOQ)
            .HasPrecision(18, 6);

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.HasOne(x => x.Department)
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SampleCategory)
            .WithMany()
            .HasForeignKey(x => x.SampleCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Matrix)
            .WithMany()
            .HasForeignKey(x => x.MatrixId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TestMethod)
            .WithMany()
            .HasForeignKey(x => x.TestMethodId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Instrument)
            .WithMany()
            .HasForeignKey(x => x.InstrumentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
