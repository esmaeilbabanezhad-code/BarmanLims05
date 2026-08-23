using Barman.Domain.Entities;

namespace Barman.Application.Interfaces;

public interface IRolePermissionService
{
    Task<List<Permission>> GetAllPermissionsAsync();

    Task<List<Permission>> GetByRoleIdAsync(Guid roleId);

    Task AssignAsync(
        Guid roleId,
        Guid permissionId);

    Task SavePermissionsAsync(
    Guid roleId,
    IEnumerable<Guid> permissionIds);
    Task RemoveAsync(Guid roleId, Guid permissionId);
}
