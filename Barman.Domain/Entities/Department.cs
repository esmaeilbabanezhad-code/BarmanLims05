using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class Department : BaseEntity
{
    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public string? Description { get; set; }

    public ICollection<TestAssignment> TestAssignments { get; set; }
        = new List<TestAssignment>();

    public ICollection<EmployeeDepartment> EmployeeDepartments { get; set; }
        = new List<EmployeeDepartment>();

    public ICollection<DepartmentResponsibility> Responsibilities { get; set; }
        = new List<DepartmentResponsibility>();
}
