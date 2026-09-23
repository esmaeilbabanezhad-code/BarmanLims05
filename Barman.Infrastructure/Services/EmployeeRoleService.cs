using Barman.Application.Interfaces;
using Barman.Domain.Entities;

namespace Barman.Infrastructure.Services;

public class EmployeeRoleService : IEmployeeRoleService
{
    private readonly IUnitOfWork _unitOfWork;

    public EmployeeRoleService(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<EmployeeRole>> GetByEmployeeIdAsync(
        Guid employeeId)
    {
        if (employeeId == Guid.Empty)
            return new List<EmployeeRole>();

        return await _unitOfWork.EmployeeRoles
            .GetByEmployeeIdAsync(employeeId);
    }
}