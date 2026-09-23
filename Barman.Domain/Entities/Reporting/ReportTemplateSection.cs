using Barman.Domain.Common;

namespace Barman.Domain.Entities.Reporting;

public class ReportTemplateSection : BaseEntity
{
    public Guid ReportTemplateId { get; set; }
    public ReportTemplate ReportTemplate { get; set; } = null!;

    public string Code { get; set; } = "";
    public string Name { get; set; } = "";

    public string SectionType { get; set; } = "Standard";
    public string Position { get; set; } = "Body";
    public string Layout { get; set; } = "Stack";

    public int DisplayOrder { get; set; }

    public bool IsVisible { get; set; } = true;

    public ICollection<ReportTemplateField> Fields { get; set; }
        = new List<ReportTemplateField>();
}