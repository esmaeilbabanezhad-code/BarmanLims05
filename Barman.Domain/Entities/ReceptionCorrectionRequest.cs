using Barman.Domain.Common;
using Barman.Domain.Enums;

namespace Barman.Domain.Entities;

public class ReceptionCorrectionRequest : BaseEntity
{
    public Guid ReceptionId { get; set; }
    public Reception Reception { get; set; } = null!;

    public Guid? SampleId { get; set; }
    public Sample? Sample { get; set; }

    public Guid RequestedByEmployeeId { get; set; }
    public Employee RequestedByEmployee { get; set; } = null!;

    public DateTimeOffset RequestedAt { get; set; } = DateTimeOffset.UtcNow;

    public string Reason { get; set; } = "";

    public ReceptionCorrectionRequestStatus Status { get; set; }
        = ReceptionCorrectionRequestStatus.Pending;

    public Guid? ResolvedByEmployeeId { get; set; }
    public Employee? ResolvedByEmployee { get; set; }

    public DateTimeOffset? ResolvedAt { get; set; }
}