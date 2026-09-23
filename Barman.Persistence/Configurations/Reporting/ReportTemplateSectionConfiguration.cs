using Barman.Domain.Entities.Reporting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Barman.Persistence.Configurations.Reporting;

public class ReportTemplateSectionConfiguration
    : IEntityTypeConfiguration<ReportTemplateSection>
{
    public void Configure(EntityTypeBuilder<ReportTemplateSection> builder)
    {
        builder.ToTable("ReportTemplateSections");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(x => x.SectionType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Position)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Layout)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.DisplayOrder)
            .IsRequired();

        builder.Property(x => x.IsVisible)
            .IsRequired();

        builder.HasOne(x => x.ReportTemplate)
            .WithMany(x => x.Sections)
            .HasForeignKey(x => x.ReportTemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new
        {
            x.ReportTemplateId,
            x.Code
        })
        .IsUnique();
    }
}