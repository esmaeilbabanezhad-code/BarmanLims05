using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface IDepartmentResponsibilityRepository
{
    Task<List<DepartmentResponsibility>> GetByEmployeeIdAsync(
        Guid employeeId);

    Task<List<DepartmentResponsibility>> GetByDepartmentIdAsync(
        Guid departmentId);

    Task<DepartmentResponsibility?> GetByIdAsync(
        Guid id);

    Task AddAsync(
        DepartmentResponsibility responsibility);

    void Update(
        DepartmentResponsibility responsibility);

    void Delete(
        DepartmentResponsibility responsibility);
}
