using Barman.Domain.Entities.Reporting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Barman.Persistence.Configurations.Reporting;

public class ReportTemplateConfiguration
    : IEntityTypeConfiguration<ReportTemplate>
{
    public void Configure(EntityTypeBuilder<ReportTemplate> builder)
    {
        builder.ToTable("ReportTemplates");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(x => x.ReportType)
            .HasMaxLength(100);

        builder.Property(x => x.Authority)
            .HasMaxLength(200);

        builder.Property(x => x.Version)
            .HasMaxLength(50);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.IsSystemTemplate)
            .IsRequired();

        builder.Property(x => x.IsSystemDefault)
            .IsRequired();

        builder.Property(x => x.CanDelete)
            .IsRequired();

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.HasIndex(x => x.IsSystemDefault);
    }
}