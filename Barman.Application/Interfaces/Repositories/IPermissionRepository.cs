using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface IPermissionRepository
{
    Task<List<Permission>> GetAllAsync();

    Task<Permission?> GetByIdAsync(Guid id);
}
