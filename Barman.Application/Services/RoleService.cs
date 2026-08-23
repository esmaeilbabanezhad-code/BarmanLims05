using Barman.Application.Interfaces;
using Barman.Domain.Entities;

namespace Barman.Application.Services;

public class RoleService : IRoleService
{
    private readonly IUnitOfWork _unitOfWork;

    public RoleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<Role>> GetAllAsync()
    {
        return await _unitOfWork.Roles.GetAllAsync();
    }

    public async Task<Role?> GetByIdAsync(Guid id)
    {
        return await _unitOfWork.Roles.GetByIdAsync(id);
    }

    public async Task<Role> CreateAsync(Role role)
    {
        var list = await _unitOfWork.Roles.GetAllAsync();

        if (list.Any(x =>
            x.Code.Equals(
                role.Code,
                StringComparison.OrdinalIgnoreCase)))
        {
            throw new Exception("Role Code already exists.");
        }

        if (list.Any(x =>
            x.Name.Equals(
                role.Name,
                StringComparison.OrdinalIgnoreCase)))
        {
            throw new Exception("Role Name already exists.");
        }

        role.IsActive = true;

        await _unitOfWork.Roles.AddAsync(role);

        await _unitOfWork.SaveChangesAsync();

        return role;
    }

    public async Task<Role?> UpdateAsync(Role role)
    {
        var current =
            await _unitOfWork.Roles.GetByIdAsync(role.Id);

        if (current == null)
            return null;

        current.Code = role.Code;
        current.Name = role.Name;
        current.Description = role.Description;
        current.IsActive = role.IsActive;

        _unitOfWork.Roles.Update(current);

        await _unitOfWork.SaveChangesAsync();

        return current;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var current =
            await _unitOfWork.Roles.GetByIdAsync(id);

        if (current == null)
            return false;

        _unitOfWork.Roles.Delete(current);

        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<Role> ActivateAsync(Guid id)
    {
        var role =
            await _unitOfWork.Roles.GetByIdAsync(id);

        if (role == null)
            throw new KeyNotFoundException(
                "Role not found.");

        role.IsActive = true;

        _unitOfWork.Roles.Update(role);

        await _unitOfWork.SaveChangesAsync();

        return role;
    }

    public async Task<Role> DeactivateAsync(Guid id)
    {
        var role =
            await _unitOfWork.Roles.GetByIdAsync(id);

        if (role == null)
            throw new KeyNotFoundException(
                "Role not found.");

        role.IsActive = false;

        _unitOfWork.Roles.Update(role);

        await _unitOfWork.SaveChangesAsync();

        return role;
    }
    public async Task<List<Role>> GetDeletedAsync()
    {
        return await _unitOfWork.Roles.GetDeletedAsync();
    }

    public async Task<Role> RestoreAsync(Guid id)
    {
        var role =
            await _unitOfWork.Roles.GetDeletedByIdAsync(id);

        if (role == null)
            throw new KeyNotFoundException(
                "Role not found.");

        role.IsDeleted = false;
        role.IsActive = true;

        _unitOfWork.Roles.Update(role);

        await _unitOfWork.SaveChangesAsync();

        return role;
    }
}