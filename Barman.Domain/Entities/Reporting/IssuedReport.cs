using Barman.Domain.Common;

namespace Barman.Domain.Entities.Reporting;

public class IssuedReport : BaseEntity
{
    public string ReportNumber { get; set; } = "";

    public int Version { get; set; } = 1;

    public Guid ReceptionId { get; set; }

    public Guid SampleId { get; set; }

    public Guid ReportTemplateId { get; set; }

    public string TemplateCode { get; set; } = "";

    public string TemplateVersion { get; set; } = "";

    public string SnapshotJson { get; set; } = "";

    public DateTime IssuedAt { get; set; }

    public Guid? IssuedByEmployeeId { get; set; }

    public bool IsCurrent { get; set; } = true;

    public bool IsCancelled { get; set; }

    public DateTime? CancelledAt { get; set; }

    public Guid? CancelledByEmployeeId { get; set; }

    public string? CancellationReason { get; set; }
}
