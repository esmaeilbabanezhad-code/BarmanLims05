using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class TestPanel : BaseEntity
{
    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public string? Description { get; set; }

    public ICollection<TestPanelItem> Items { get; set; }
        = new List<TestPanelItem>();
}