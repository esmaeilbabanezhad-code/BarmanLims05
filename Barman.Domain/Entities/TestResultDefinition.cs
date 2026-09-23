using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class TestResultDefinition : BaseEntity
{
    public Guid TestId { get; set; }

    public Test Test { get; set; } = null!;

    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public string? Unit { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsRequired { get; set; } = true;

    public int? DecimalPlaces { get; set; }

    public string? Description { get; set; }
}