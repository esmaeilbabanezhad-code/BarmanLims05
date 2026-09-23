using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class DefaultTestSet : BaseEntity
{
    // =========================
    // Identity
    // =========================

    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public string? Description { get; set; }

    // =========================
    // Optional Customer
    // =========================

    public Guid? CustomerId { get; set; }

    public Customer? Customer { get; set; }

    // =========================
    // Optional Sample Category
    // =========================

    public Guid? SampleCategoryId { get; set; }

    public SampleCategory? SampleCategory { get; set; }

    // =========================
    // Optional Matrix
    // =========================

    public Guid? MatrixId { get; set; }

    public Matrix? Matrix { get; set; }

    // =========================
    // Optional Standard Sample
    // =========================

    public Guid? StandardSampleId { get; set; }

    public StandardSample? StandardSample { get; set; }

    // =========================
    // Items
    // =========================

    public ICollection<DefaultTestSetItem> Items { get; set; }
        = new List<DefaultTestSetItem>();

    // =========================
    // Settings
    // =========================

    public int Priority { get; set; } = 100;
}