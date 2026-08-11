using Barman.Domain.Common;

namespace Barman.Domain.Entities;

public class Permission : BaseEntity
{
    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public string? Description { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; }
        = new List<RolePermission>();
}