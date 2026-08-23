using Barman.Domain.Entities;

namespace Barman.Application.Interfaces;

public interface IRoleService
{
    Task<List<Role>> GetAllAsync();

    Task<Role?> GetByIdAsync(Guid id);

    Task<Role> CreateAsync(Role role);

    Task<Role?> UpdateAsync(Role role);

    Task<bool> DeleteAsync(Guid id);

    Task<List<Role>> GetDeletedAsync();

    Task<Role> RestoreAsync(Guid id);

    Task<Role> ActivateAsync(Guid id);

    Task<Role> DeactivateAsync(Guid id);
}
