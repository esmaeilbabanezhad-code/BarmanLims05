using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class DepartmentResponsibility : BaseEntity
{
    public Guid EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;

    public Guid DepartmentId { get; set; }

    public Department Department { get; set; } = null!;

    public DepartmentResponsibilityType ResponsibilityType { get; set; }

    public bool IsPrimary { get; set; } = false;
}
