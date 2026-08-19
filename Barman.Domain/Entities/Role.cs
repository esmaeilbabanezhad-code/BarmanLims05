using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class Role : BaseEntity
{
    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public string? Description { get; set; }

    public ICollection<EmployeeRole> EmployeeRoles { get; set; }
        = new List<EmployeeRole>();

    public ICollection<RolePermission> RolePermissions { get; set; }
        = new List<RolePermission>();
}