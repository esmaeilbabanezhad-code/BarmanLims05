using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface IEmployeeRepository
{
    Task AddAsync(Employee employee);

    Task<Employee?> GetByIdAsync(Guid id);

    Task<List<Employee>> GetAllAsync();

    Task<List<Employee>> GetByDepartmentAsync(Guid departmentId);

    void Update(Employee employee);

    void Delete(Employee employee);
}