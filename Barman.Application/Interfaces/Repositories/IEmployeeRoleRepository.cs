using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface IEmployeeRoleRepository
{
    Task AddAsync(EmployeeRole employeeRole);

    Task<List<EmployeeRole>> GetByEmployeeIdAsync(Guid employeeId);

    Task<EmployeeRole?> GetByIdAsync(Guid id);

    void Update(EmployeeRole employeeRole);

    void Delete(EmployeeRole employeeRole);
}