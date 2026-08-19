using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class EmployeeRole : BaseEntity
{
    public Guid EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;

    public Guid RoleId { get; set; }

    public Role Role { get; set; } = null!;

    public bool IsPrimary { get; set; }

}