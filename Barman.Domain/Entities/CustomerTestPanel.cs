using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class CustomerTestPanel : BaseEntity
{
    // =========================
    // Customer
    // =========================

    public Guid CustomerId { get; set; }

    public Customer Customer { get; set; } = null!;

    // =========================
    // Sample Category (Optional)
    // =========================

    public Guid? SampleCategoryId { get; set; }

    public SampleCategory? SampleCategory { get; set; }

    // =========================
    // Matrix (Optional)
    // =========================

    public Guid? MatrixId { get; set; }

    public Matrix? Matrix { get; set; }

    // =========================
    // Test Panel
    // =========================

    public Guid TestPanelId { get; set; }

    public TestPanel TestPanel { get; set; } = null!;

    // =========================
    // Rule Settings
    // =========================

    public int Priority { get; set; } = 100;

    public string? Description { get; set; }
}