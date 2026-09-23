using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Barman.Domain.Entities;

namespace Barman.Persistence.Configurations;

public class TestTariffConfiguration : IEntityTypeConfiguration<TestTariff>
{
    public void Configure(EntityTypeBuilder<TestTariff> builder)
    {
        builder.ToTable("TestTariffs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Currency)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.Price)
            .HasPrecision(18, 2);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.HasIndex(x => new
        {
            x.OrganizationTypeId,
            x.TestId,
            x.TestPanelId,
            x.CustomerId,
            x.SampleCategoryId,
            x.StandardSampleId,
            x.ValidFrom
        });

        builder.HasOne(x => x.OrganizationType)
            .WithMany(x => x.TestTariffs)
            .HasForeignKey(x => x.OrganizationTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Test)
            .WithMany()
            .HasForeignKey(x => x.TestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TestPanel)
            .WithMany()
            .HasForeignKey(x => x.TestPanelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.StandardSample)
            .WithMany()
            .HasForeignKey(x => x.StandardSampleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SampleCategory)
            .WithMany()
            .HasForeignKey(x => x.SampleCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Matrix)
            .WithMany()
            .HasForeignKey(x => x.MatrixId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}