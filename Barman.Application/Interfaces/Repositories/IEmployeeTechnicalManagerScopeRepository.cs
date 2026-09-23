using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface IEmployeeTechnicalManagerScopeRepository
{
    Task<List<EmployeeTechnicalManagerScope>> GetByEmployeeIdAsync(
        Guid employeeId);

    Task<List<EmployeeTechnicalManagerScope>> GetByScopeIdAsync(
        Guid scopeId);

    Task<EmployeeTechnicalManagerScope?> GetByIdAsync(
        Guid id);

    Task AddAsync(
        EmployeeTechnicalManagerScope scope);

    void Update(
        EmployeeTechnicalManagerScope scope);

    void Delete(
        EmployeeTechnicalManagerScope scope);
}