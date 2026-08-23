using Barman.Application.Interfaces;

namespace Barman.Application.Services;

public class PermissionService : IPermissionService
{
    private const string AdminPermissionCode = "Admin";

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public PermissionService(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<bool> HasPermissionAsync(
        Guid employeeId,
        string permissionCode)
    {
        if (employeeId == Guid.Empty ||
            string.IsNullOrWhiteSpace(permissionCode))
        {
            return false;
        }

        var employeeRoles =
            await _unitOfWork.EmployeeRoles
                .GetWithPermissionsByEmployeeIdAsync(employeeId);

        Console.WriteLine(
    $"[PermissionService] EmployeeId={employeeId}, " +
    $"Permission={permissionCode}, " +
    $"RolesCount={employeeRoles.Count}");

        var permissions = employeeRoles
            .Where(x => x.Role != null)
            .SelectMany(x => x.Role.RolePermissions)
            .Where(x => x.Permission != null)
            .Select(x => x.Permission.Code)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        Console.WriteLine(
    $"[PermissionService] Permissions={string.Join(", ", permissions)}");

        if (permissions.Contains(AdminPermissionCode))
            return true;

        return permissions.Contains(permissionCode.Trim());
    }

    public async Task<bool> HasPermissionAsync(
        string permissionCode)
    {
        var employeeId = _currentUserService.EmployeeId;
        Console.WriteLine(
            $"[PermissionService] CurrentUser InstanceId={_currentUserService.InstanceId}, EmployeeId={employeeId}");

        if (!employeeId.HasValue)
            return false;

        return await HasPermissionAsync(
            employeeId.Value,
            permissionCode);
    }

    public async Task<List<string>> GetPermissionCodesAsync(
        Guid employeeId)
    {
        if (employeeId == Guid.Empty)
            return new List<string>();

        var employeeRoles =
            await _unitOfWork.EmployeeRoles
                .GetWithPermissionsByEmployeeIdAsync(employeeId);

        var permissions = employeeRoles
            .Where(x => x.Role != null)
            .SelectMany(x => x.Role.RolePermissions)
            .Where(x => x.Permission != null)
            .Select(x => x.Permission.Code)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x)
            .ToList();

        return permissions;
    }

    public async Task<List<string>> GetCurrentUserPermissionCodesAsync()
    {
        var employeeId = _currentUserService.EmployeeId;

        if (!employeeId.HasValue)
            return new List<string>();

        return await GetPermissionCodesAsync(employeeId.Value);
    }
    public async Task<string> DebugEmployeeRolesAsync(Guid employeeId)
    {
        if (employeeId == Guid.Empty)
            return "EmployeeId is empty";

        var employeeRoles =
            await _unitOfWork.EmployeeRoles
                .GetWithPermissionsByEmployeeIdAsync(employeeId);

        var result = new List<string>();

        foreach (var employeeRole in employeeRoles)
        {
            var roleCode =
                employeeRole.Role?.Code ?? "NULL_ROLE";

            var permissions =
                employeeRole.Role?.RolePermissions?
                    .Where(x => x.Permission != null)
                    .Select(x => x.Permission.Code)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToList()
                ?? new List<string>();

            result.Add(
                $"Role={roleCode} | Permissions=[{string.Join(", ", permissions)}]");
        }

        return
            $"RolesCount={employeeRoles.Count} || " +
            string.Join(" || ", result);
    }
}