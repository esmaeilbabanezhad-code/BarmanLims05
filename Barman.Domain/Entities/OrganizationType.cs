using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class OrganizationType : BaseEntity
{
    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public string? Description { get; set; }

    public int DisplayOrder { get; set; }

    public ICollection<TestTariff> TestTariffs { get; set; }
        = new List<TestTariff>();
}