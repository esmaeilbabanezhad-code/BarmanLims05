using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(Guid id);

    Task<List<Role>> GetAllAsync();

    Task<List<Role>> GetActiveAsync();

    Task<List<Role>> GetDeletedAsync();

    Task<Role?> GetDeletedByIdAsync(Guid id);

    Task AddAsync(Role role);

    void Update(Role role);

    void Delete(Role role);
}
