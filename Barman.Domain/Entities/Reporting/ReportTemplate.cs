using Barman.Domain.Common;

namespace Barman.Domain.Entities.Reporting;

public class ReportTemplate : BaseEntity
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string? ReportType { get; set; }
    public string? Authority { get; set; }
    public string? Version { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsSystemTemplate { get; set; }

    public bool IsSystemDefault { get; set; }

    public bool CanDelete { get; set; } = true;

    public string? Description { get; set; }

    public ICollection<ReportTemplateSection> Sections { get; set; }
        = new List<ReportTemplateSection>();
}