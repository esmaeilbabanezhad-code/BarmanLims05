using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class DefaultTestSetItem : BaseEntity
{
    // =========================
    // Default Test Set
    // =========================

    public Guid DefaultTestSetId { get; set; }

    public DefaultTestSet DefaultTestSet { get; set; } = null!;

    // =========================
    // Item Type
    // =========================

    public bool IsPanel { get; set; }

    // =========================
    // Independent Test
    // =========================

    public Guid? TestId { get; set; }

    public Test? Test { get; set; }

    // =========================
    // Test Panel
    // =========================

    public Guid? TestPanelId { get; set; }

    public TestPanel? TestPanel { get; set; }

    // =========================
    // Order
    // =========================

    public int SortOrder { get; set; } = 0;
}