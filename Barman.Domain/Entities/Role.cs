using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class Role : BaseEntity
{
    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public string? Description { get; set; }

    public ICollection<Employee> Employees { get; set; }
        = new List<Employee>();

    public ICollection<RolePermission> RolePermissions { get; set; }
        = new List<RolePermission>();
}