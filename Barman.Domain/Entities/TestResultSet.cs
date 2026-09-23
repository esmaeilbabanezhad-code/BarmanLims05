using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class TestResultSet : BaseEntity
{
    // =========================
    // Test
    // =========================

    public Guid TestId { get; set; }

    public Test Test { get; set; } = null!;

    // =========================
    // Scope - Customer
    // =========================

    public Guid? CustomerId { get; set; }

    public Customer? Customer { get; set; }

    // =========================
    // Scope - Sample Category
    // =========================

    public Guid? SampleCategoryId { get; set; }

    public SampleCategory? SampleCategory { get; set; }

    // =========================
    // Scope - Matrix
    // =========================

    public Guid? MatrixId { get; set; }

    public Matrix? Matrix { get; set; }

    // =========================
    // Scope - Standard Sample
    // =========================

    public Guid? StandardSampleId { get; set; }

    public StandardSample? StandardSample { get; set; }

    // =========================
    // Identity
    // =========================

    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public string? Description { get; set; }

    // =========================
    // Selection Priority
    // =========================

    public int Priority { get; set; } = 100;

    // =========================
    // Items
    // =========================

    public ICollection<TestResultSetItem> Items { get; set; }
        = new List<TestResultSetItem>();
}