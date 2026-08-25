using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class StandardSample : BaseEntity
{
    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public Guid SampleCategoryId { get; set; }

    public SampleCategory SampleCategory { get; set; } = null!;

    public Guid MatrixId { get; set; }

    public Matrix Matrix { get; set; } = null!;

    public string? Description { get; set; }

    public ICollection<Sample> Samples { get; set; }
        = new List<Sample>();
}