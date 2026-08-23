namespace Barman.Application.Interfaces;

public interface IPermissionService
{
    Task<bool> HasPermissionAsync(
        Guid employeeId,
        string permissionCode);

    Task<bool> HasPermissionAsync(
        string permissionCode);

    Task<List<string>> GetPermissionCodesAsync(
        Guid employeeId);

    Task<List<string>> GetCurrentUserPermissionCodesAsync();

    Task<string> DebugEmployeeRolesAsync(
        Guid employeeId);
}