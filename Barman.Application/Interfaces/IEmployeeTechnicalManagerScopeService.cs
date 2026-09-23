using Barman.Domain.Entities;

namespace Barman.Application.Interfaces;

public interface IEmployeeTechnicalManagerScopeService
{
    Task<List<EmployeeTechnicalManagerScope>> GetByEmployeeIdAsync(
        Guid employeeId);

    Task<List<EmployeeTechnicalManagerScope>> GetByScopeIdAsync(
        Guid scopeId);

    Task<EmployeeTechnicalManagerScope?> GetByIdAsync(
        Guid id);

    Task<EmployeeTechnicalManagerScope> AddAsync(
        EmployeeTechnicalManagerScope scope);

    Task<bool> UpdateAsync(
        EmployeeTechnicalManagerScope scope);

    Task<bool> DeleteAsync(
        Guid id);

    Task<bool> ActivateAsync(
        Guid id);
}