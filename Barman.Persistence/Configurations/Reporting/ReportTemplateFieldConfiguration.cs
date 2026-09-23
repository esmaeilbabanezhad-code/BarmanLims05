using Barman.Domain.Entities.Reporting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Barman.Persistence.Configurations.Reporting;

public class ReportTemplateFieldConfiguration
    : IEntityTypeConfiguration<ReportTemplateField>
{
    public void Configure(EntityTypeBuilder<ReportTemplateField> builder)
    {
        builder.ToTable("ReportTemplateFields");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Source)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.FieldCode)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.FieldType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Caption)
            .HasMaxLength(300);

        builder.Property(x => x.DisplayOrder)
            .IsRequired();

        builder.Property(x => x.Width)
            .HasMaxLength(50);

        builder.Property(x => x.Alignment)
            .HasMaxLength(30);

        builder.Property(x => x.Format)
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.HasOne(x => x.ReportTemplateSection)
            .WithMany(x => x.Fields)
            .HasForeignKey(x => x.ReportTemplateSectionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new
        {
            x.ReportTemplateSectionId,
            x.FieldCode
        })
        .IsUnique();
    }
}
