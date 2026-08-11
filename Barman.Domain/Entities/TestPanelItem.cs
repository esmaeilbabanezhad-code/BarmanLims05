using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class TestPanelItem : BaseEntity
{
    public Guid TestPanelId { get; set; }

    public TestPanel TestPanel { get; set; } = null!;

    public Guid TestId { get; set; }

    public Test Test { get; set; } = null!;

    public int SortOrder { get; set; }
}