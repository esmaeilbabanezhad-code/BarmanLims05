using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class LimitReference : BaseEntity
{
    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public string? Version { get; set; }

    public DateTime? IssueDate { get; set; }

    public DateTime? ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public bool IsActive { get; set; } = true;

    public string? DocumentNo { get; set; }

    public int Priority { get; set; }

    public string? Description { get; set; }

    public ICollection<TestLimitRule> TestLimitRules { get; set; }
        = new List<TestLimitRule>();
}