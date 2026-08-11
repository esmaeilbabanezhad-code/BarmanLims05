using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class Sample : BaseEntity
{
    public Guid ReceptionId { get; set; }

    public Reception Reception { get; set; } = null!;

    public string SampleCode { get; set; } = "";

    public string SampleName { get; set; } = "";

    public Guid? SampleCategoryId { get; set; }

    public SampleCategory? SampleCategory { get; set; }

    public string? Matrix { get; set; }

    public decimal? Quantity { get; set; }

    public string? Unit { get; set; }

    public string? ContainerType { get; set; }

    public string? Description { get; set; }

    public ICollection<TestAssignment> TestAssignments { get; set; }
        = new List<TestAssignment>();
}