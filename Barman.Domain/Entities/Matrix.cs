using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class Matrix : BaseEntity
{
    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public Guid SampleCategoryId { get; set; }

    public SampleCategory SampleCategory { get; set; } = null!;

    public string? Description { get; set; }
}