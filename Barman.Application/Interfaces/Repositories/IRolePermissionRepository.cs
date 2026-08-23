using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface IRolePermissionRepository
{
    Task AddAsync(RolePermission rolePermission);

    Task<List<RolePermission>> GetByRoleIdAsync(Guid roleId);

    Task<RolePermission?> GetByIdAsync(Guid id);

    void Delete(RolePermission rolePermission);
}
