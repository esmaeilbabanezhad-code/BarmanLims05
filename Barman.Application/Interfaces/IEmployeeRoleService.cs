using Barman.Domain.Entities;

namespace Barman.Application.Interfaces;

public interface IEmployeeRoleService
{
    Task<List<EmployeeRole>> GetByEmployeeIdAsync(Guid employeeId);
}