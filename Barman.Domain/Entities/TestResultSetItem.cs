using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class TestResultSetItem : BaseEntity
{
    // =========================
    // Result Set
    // =========================

    public Guid TestResultSetId { get; set; }

    public TestResultSet TestResultSet { get; set; } = null!;

    // =========================
    // Result Definition
    // =========================

    public Guid TestResultDefinitionId { get; set; }

    public TestResultDefinition TestResultDefinition { get; set; } = null!;

    // =========================
    // Display Order
    // =========================

    public int DisplayOrder { get; set; }

    public decimal? LOD { get; set; }

    public decimal? LOQ { get; set; }

    public decimal? MinValue { get; set; }

    public decimal? MaxValue { get; set; }
}