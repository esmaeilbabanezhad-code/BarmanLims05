using Barman.Domain.Common;
using Barman.Domain.Enums;

namespace Barman.Domain.Entities;

public class TestLimitRule : BaseEntity
{
    public Guid TestId { get; set; }

    public Test Test { get; set; } = null!;

    public Guid? MatrixId { get; set; }

    public Matrix? Matrix { get; set; }

    public Guid? SampleCategoryId { get; set; }

    public SampleCategory? SampleCategory { get; set; }

    public Guid? CustomerId { get; set; }

    public Customer? Customer { get; set; }

    public Guid LimitReferenceId { get; set; }

    public LimitReference LimitReference { get; set; } = null!;

    public LimitType LimitType { get; set; }

    public decimal? LowerValue { get; set; }

    public decimal? UpperValue { get; set; }

    public decimal? ExactValue { get; set; }

    public bool LowerInclusive { get; set; } = true;

    public bool UpperInclusive { get; set; } = true;

    public string? AllowedValues { get; set; }

    public string? Unit { get; set; }

    public int Priority { get; set; }

    public DateTime ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public bool IsActive { get; set; } = true;

    public string? Description { get; set; }
}
