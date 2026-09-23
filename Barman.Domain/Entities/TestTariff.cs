using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class TestTariff : BaseEntity
{
    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public Guid? TestId { get; set; }

    public Test? Test { get; set; }

    public Guid? TestPanelId { get; set; }

    public TestPanel? TestPanel { get; set; }

    public Guid OrganizationTypeId { get; set; }

    public OrganizationType OrganizationType { get; set; } = null!;

    public Guid? CustomerId { get; set; }

    public Customer? Customer { get; set; }

    public Guid? StandardSampleId { get; set; }

    public StandardSample? StandardSample { get; set; }

    public Guid? SampleCategoryId { get; set; }

    public SampleCategory? SampleCategory { get; set; }

    public Guid? MatrixId { get; set; }

    public Matrix? Matrix { get; set; }

    public decimal Price { get; set; }

    public string Currency { get; set; } = "ریال";

    public DateTime ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public int Priority { get; set; } = 100;

    public string? Description { get; set; }
}