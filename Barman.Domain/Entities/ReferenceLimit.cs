using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class ReferenceLimit : BaseEntity
{
    public Guid TestId { get; set; }

    public Test Test { get; set; } = null!;

    public Guid? MatrixId { get; set; }

    public Matrix? Matrix { get; set; }

    public string? ProductName { get; set; }

    public string? OrganizationName { get; set; }

    public decimal? MinValue { get; set; }

    public decimal? MaxValue { get; set; }

    public decimal? WarningLow { get; set; }

    public decimal? WarningHigh { get; set; }

    public string? Unit { get; set; }

    public int Priority { get; set; }

    public bool IsTemporary { get; set; }

    public DateTime? ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public string? Description { get; set; }
}