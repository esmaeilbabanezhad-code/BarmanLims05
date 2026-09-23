using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface IOrganizationTypeRepository
{
    Task<OrganizationType?> GetByIdAsync(Guid id);

    Task<List<OrganizationType>> GetAllAsync();

    Task<List<OrganizationType>> GetDeletedAsync();

    Task<OrganizationType?> GetDeletedByIdAsync(Guid id);

    Task AddAsync(OrganizationType organizationType);

    void Update(OrganizationType organizationType);

    void Delete(OrganizationType organizationType);
}