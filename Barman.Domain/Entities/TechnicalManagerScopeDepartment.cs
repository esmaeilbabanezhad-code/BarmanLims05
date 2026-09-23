using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class TechnicalManagerScopeDepartment : BaseEntity
{
    public Guid TechnicalManagerScopeId { get; set; }
    public TechnicalManagerScope TechnicalManagerScope { get; set; } = null!;

    public Guid DepartmentId { get; set; }
    public Department Department { get; set; } = null!;

    public int Priority { get; set; } = 100;

    public bool IsPrimary { get; set; }
}
