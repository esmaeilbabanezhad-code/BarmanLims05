using Barman.Application.Interfaces;
using Barman.Domain.Entities;

namespace Barman.Application.Services;

public class EmployeeTechnicalManagerScopeService
    : IEmployeeTechnicalManagerScopeService
{
    private readonly IUnitOfWork _unitOfWork;

    public EmployeeTechnicalManagerScopeService(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<EmployeeTechnicalManagerScope>> GetByEmployeeIdAsync(
        Guid employeeId)
    {
        if (employeeId == Guid.Empty)
            return new List<EmployeeTechnicalManagerScope>();

        return await _unitOfWork.EmployeeTechnicalManagerScopes
            .GetByEmployeeIdAsync(employeeId);
    }

    public async Task<List<EmployeeTechnicalManagerScope>> GetByScopeIdAsync(
        Guid scopeId)
    {
        if (scopeId == Guid.Empty)
            return new List<EmployeeTechnicalManagerScope>();

        return await _unitOfWork.EmployeeTechnicalManagerScopes
            .GetByScopeIdAsync(scopeId);
    }

    public async Task<EmployeeTechnicalManagerScope?> GetByIdAsync(
        Guid id)
    {
        if (id == Guid.Empty)
            return null;

        return await _unitOfWork.EmployeeTechnicalManagerScopes
            .GetByIdAsync(id);
    }

    public async Task<EmployeeTechnicalManagerScope> AddAsync(
        EmployeeTechnicalManagerScope scope)
    {
        if (scope == null)
            throw new ArgumentNullException(nameof(scope));

        if (scope.EmployeeId == Guid.Empty)
            throw new InvalidOperationException(
                "کارمند مسئول فنی الزامی است.");

        if (scope.TechnicalManagerScopeId == Guid.Empty)
            throw new InvalidOperationException(
                "محدوده مسئول فنی الزامی است.");

        var existingScopes =
            await _unitOfWork.EmployeeTechnicalManagerScopes
                .GetByEmployeeIdAsync(scope.EmployeeId);

        var duplicate = existingScopes.Any(x =>
            !x.IsDeleted &&
            x.TechnicalManagerScopeId == scope.TechnicalManagerScopeId);

        if (duplicate)
            throw new InvalidOperationException(
                "این محدوده قبلاً به این کارمند اختصاص داده شده است.");

        if (scope.IsPrimary)
        {
            var scopeMembers =
                await _unitOfWork.EmployeeTechnicalManagerScopes
                    .GetByScopeIdAsync(scope.TechnicalManagerScopeId);

            foreach (var item in scopeMembers.Where(x =>
                !x.IsDeleted &&
                x.IsPrimary))
            {
                item.IsPrimary = false;
                item.ModifiedAt = DateTimeOffset.UtcNow;

                _unitOfWork.EmployeeTechnicalManagerScopes
                    .Update(item);
            }
        }

        scope.IsDeleted = false;
        scope.IsActive = true;

        await _unitOfWork.EmployeeTechnicalManagerScopes
            .AddAsync(scope);

        await _unitOfWork.SaveChangesAsync();

        return scope;
    }

    public async Task<bool> UpdateAsync(
        EmployeeTechnicalManagerScope scope)
    {
        if (scope == null)
            throw new ArgumentNullException(nameof(scope));

        if (scope.Id == Guid.Empty)
            throw new InvalidOperationException(
                "شناسه تخصیص محدوده نامعتبر است.");

        if (scope.EmployeeId == Guid.Empty)
            throw new InvalidOperationException(
                "کارمند مسئول فنی الزامی است.");

        if (scope.TechnicalManagerScopeId == Guid.Empty)
            throw new InvalidOperationException(
                "محدوده مسئول فنی الزامی است.");

        var existing =
            await _unitOfWork.EmployeeTechnicalManagerScopes
                .GetByIdAsync(scope.Id);

        if (existing == null)
            return false;

        var employeeScopes =
            await _unitOfWork.EmployeeTechnicalManagerScopes
                .GetByEmployeeIdAsync(scope.EmployeeId);

        var duplicate = employeeScopes.Any(x =>
            !x.IsDeleted &&
            x.Id != scope.Id &&
            x.TechnicalManagerScopeId == scope.TechnicalManagerScopeId);

        if (duplicate)
            throw new InvalidOperationException(
                "این محدوده قبلاً به این کارمند اختصاص داده شده است.");

        if (scope.IsPrimary)
        {
            var scopeMembers =
                await _unitOfWork.EmployeeTechnicalManagerScopes
                    .GetByScopeIdAsync(scope.TechnicalManagerScopeId);

            foreach (var item in scopeMembers.Where(x =>
                !x.IsDeleted &&
                x.Id != scope.Id &&
                x.IsPrimary))
            {
                item.IsPrimary = false;
                item.ModifiedAt = DateTimeOffset.UtcNow;

                _unitOfWork.EmployeeTechnicalManagerScopes
                    .Update(item);
            }
        }

        existing.EmployeeId = scope.EmployeeId;
        existing.TechnicalManagerScopeId =
            scope.TechnicalManagerScopeId;
        existing.IsPrimary = scope.IsPrimary;
        existing.IsActive = scope.IsActive;
        existing.IsDeleted = scope.IsDeleted;
        existing.ModifiedAt = DateTimeOffset.UtcNow;

        _unitOfWork.EmployeeTechnicalManagerScopes
            .Update(existing);

        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        if (id == Guid.Empty)
            return false;

        var scope =
            await _unitOfWork.EmployeeTechnicalManagerScopes
                .GetByIdAsync(id);

        if (scope == null)
            return false;

        scope.IsDeleted = true;
        scope.IsActive = false;
        scope.ModifiedAt = DateTimeOffset.UtcNow;

        _unitOfWork.EmployeeTechnicalManagerScopes
            .Update(scope);

        await _unitOfWork.SaveChangesAsync();

        return true;
    }
    public async Task<bool> ActivateAsync(Guid id)
    {
        if (id == Guid.Empty)
            return false;

        var scope =
            await _unitOfWork.EmployeeTechnicalManagerScopes
                .GetByIdAsync(id);

        if (scope == null)
            return false;

        scope.IsDeleted = false;
        scope.IsActive = true;
        scope.ModifiedAt = DateTimeOffset.UtcNow;

        _unitOfWork.EmployeeTechnicalManagerScopes
            .Update(scope);

        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
