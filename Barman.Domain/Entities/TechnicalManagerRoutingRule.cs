using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class TechnicalManagerRoutingRule : BaseEntity
{
    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public Guid? CustomerId { get; set; }

    public Customer? Customer { get; set; }

    public Guid? SampleCategoryId { get; set; }

    public SampleCategory? SampleCategory { get; set; }

    public Guid? MatrixId { get; set; }

    public Matrix? Matrix { get; set; }

    public Guid? TestPanelId { get; set; }

    public TestPanel? TestPanel { get; set; }

    public Guid? TestId { get; set; }

    public Test? Test { get; set; }

    public Guid? DepartmentId { get; set; }

    public Department? Department { get; set; }

    public Guid TechnicalManagerScopeId { get; set; }

    public TechnicalManagerScope TechnicalManagerScope { get; set; } = null!;

    public int Priority { get; set; } = 100;

    
}