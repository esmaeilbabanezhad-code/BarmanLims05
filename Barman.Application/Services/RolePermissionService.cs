using Barman.Application.Interfaces;
using Barman.Domain.Entities;

namespace Barman.Application.Services;

public class RolePermissionService : IRolePermissionService
{
    private readonly IUnitOfWork _unitOfWork;

    public RolePermissionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<Permission>> GetAllPermissionsAsync()
    {
        return await _unitOfWork.Permissions
            .GetAllAsync();
    }

    public async Task<List<Permission>> GetByRoleIdAsync(Guid roleId)
    {
        var rolePermissions =
            await _unitOfWork.RolePermissions
                .GetByRoleIdAsync(roleId);

        return rolePermissions
            .Where(x => x.Permission != null)
            .Select(x => x.Permission)
            .ToList();
    }

    public async Task AssignAsync(
        Guid roleId,
        Guid permissionId)
    {
        var existing =
            (await _unitOfWork.RolePermissions
                .GetByRoleIdAsync(roleId))
            .FirstOrDefault(x =>
                x.PermissionId == permissionId);

        if (existing != null)
            return;

        await _unitOfWork.RolePermissions.AddAsync(
            new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId
            });

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task SavePermissionsAsync(
    Guid roleId,
    IEnumerable<Guid> permissionIds)
    {
        var selectedIds = permissionIds.ToHashSet();

        var existing =
            await _unitOfWork.RolePermissions
                .GetByRoleIdAsync(roleId);

        var existingIds =
            existing
                .Select(x => x.PermissionId)
                .ToHashSet();

        // افزودن Permissionهای جدید
        foreach (var permissionId in selectedIds.Except(existingIds))
        {
            await _unitOfWork.RolePermissions.AddAsync(
                new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permissionId
                });
        }

        // حذف Permissionهایی که دیگر انتخاب نشده‌اند
        foreach (var rolePermission in existing
            .Where(x => !selectedIds.Contains(x.PermissionId)))
        {
            _unitOfWork.RolePermissions.Delete(rolePermission);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RemoveAsync(
        Guid roleId,
        Guid permissionId)
    {
        var existing =
            (await _unitOfWork.RolePermissions
                .GetByRoleIdAsync(roleId))
            .FirstOrDefault(x =>
                x.PermissionId == permissionId);

        if (existing == null)
            return;

        _unitOfWork.RolePermissions.Delete(existing);

        await _unitOfWork.SaveChangesAsync();
    }
}

