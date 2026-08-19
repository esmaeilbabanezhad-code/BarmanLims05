using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class EmployeeDepartment : BaseEntity
{
    public Guid EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;

    public Guid DepartmentId { get; set; }

    public Department Department { get; set; } = null!;

    public bool IsPrimary { get; set; }

}