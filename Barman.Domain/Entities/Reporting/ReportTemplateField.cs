using Barman.Domain.Common;

namespace Barman.Domain.Entities.Reporting;

public class ReportTemplateField : BaseEntity
{
    public Guid ReportTemplateSectionId { get; set; }
    public ReportTemplateSection ReportTemplateSection { get; set; } = null!;

    // Data source
    public string Source { get; set; } = "";

    // Field identifier inside the source
    public string FieldCode { get; set; } = "";

    // Display type
    public string FieldType { get; set; } = "Value";

    public string? Caption { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsVisible { get; set; } = true;

    // Layout
    public string? Width { get; set; }
    public string? Alignment { get; set; }

    // Formatting
    public string? Format { get; set; }

    public string? Description { get; set; }
}