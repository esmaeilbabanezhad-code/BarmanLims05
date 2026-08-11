using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class TestMethod : BaseEntity
{
    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public string? StandardReference { get; set; }

    public string? SOPCode { get; set; }

    public string? Description { get; set; }
}