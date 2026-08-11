using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class SampleCategory : BaseEntity
{
    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public string? Description { get; set; }

    public ICollection<Sample> Samples { get; set; }
        = new List<Sample>();
}