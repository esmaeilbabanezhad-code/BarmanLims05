using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class Department : BaseEntity
{
    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public string? Description { get; set; }

    public ICollection<TestAssignment> TestAssignments { get; set; }
        = new List<TestAssignment>();

    public ICollection<Employee> Employees { get; set; }
    = new List<Employee>();
}