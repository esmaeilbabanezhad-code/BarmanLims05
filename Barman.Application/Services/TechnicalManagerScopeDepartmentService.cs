using Barman.Application.Interfaces;
using Barman.Domain.Entities;

namespace Barman.Application.Services;

public class TechnicalManagerScopeDepartmentService
    : ITechnicalManagerScopeDepartmentService
{
    private readonly IUnitOfWork _unitOfWork;

    public TechnicalManagerScopeDepartmentService(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TechnicalManagerScopeDepartment>>
        GetByScopeIdAsync(
            Guid scopeId,
            CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.TechnicalManagerScopeDepartments
            .GetByScopeIdAsync(scopeId);
    }

    public async Task<List<TechnicalManagerScopeDepartment>>
        GetByDepartmentIdAsync(
            Guid departmentId,
            CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.TechnicalManagerScopeDepartments
            .GetByDepartmentIdAsync(departmentId);
    }

    public async Task<TechnicalManagerScopeDepartment?>
        GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.TechnicalManagerScopeDepartments
            .GetByIdAsync(id);
    }

    public async Task<TechnicalManagerScopeDepartment> AddAsync(
        TechnicalManagerScopeDepartment relation,
        CancellationToken cancellationToken = default)
    {
        var scope =
            await _unitOfWork.TechnicalManagerScopes
                .GetByIdAsync(
                    relation.TechnicalManagerScopeId);

        if (scope is null ||
            scope.IsDeleted ||
            !scope.IsActive)
        {
            throw new InvalidOperationException(
                "The technical manager scope is not active.");
        }

        var department =
            await _unitOfWork.Departments
                .GetByIdAsync(
                    relation.DepartmentId);

        if (department is null ||
            department.IsDeleted ||
            !department.IsActive)
        {
            throw new InvalidOperationException(
                "The department is not active.");
        }

        var exists =
            await _unitOfWork.TechnicalManagerScopeDepartments
                .ExistsAsync(
                    relation.TechnicalManagerScopeId,
                    relation.DepartmentId);

        if (exists)
        {
            throw new InvalidOperationException(
                "This department is already assigned to this technical manager scope.");
        }

        var existing =
            await _unitOfWork.TechnicalManagerScopeDepartments
                .GetByScopeIdAsync(
                    relation.TechnicalManagerScopeId);

        if (!existing.Any())
        {
            relation.IsPrimary = true;
        }

        if (relation.IsPrimary)
        {
            foreach (var item in existing)
            {
                item.IsPrimary = false;
                _unitOfWork.TechnicalManagerScopeDepartments
                    .Update(item);
            }
        }

        relation.IsActive = true;
        relation.IsDeleted = false;

        await _unitOfWork.TechnicalManagerScopeDepartments
            .AddAsync(relation);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return relation;
    }

    public async Task<bool> UpdateAsync(
        TechnicalManagerScopeDepartment relation,
        CancellationToken cancellationToken = default)
    {
        var existing =
            await _unitOfWork.TechnicalManagerScopeDepartments
                .GetByIdAsync(relation.Id);

        if (existing is null)
            return false;

        var scope =
            await _unitOfWork.TechnicalManagerScopes
                .GetByIdAsync(
                    relation.TechnicalManagerScopeId);

        if (scope is null ||
            scope.IsDeleted ||
            !scope.IsActive)
        {
            throw new InvalidOperationException(
                "The technical manager scope is not active.");
        }

        var department =
            await _unitOfWork.Departments
                .GetByIdAsync(
                    relation.DepartmentId);

        if (department is null ||
            department.IsDeleted ||
            !department.IsActive)
        {
            throw new InvalidOperationException(
                "The department is not active.");
        }

        var exists =
            await _unitOfWork.TechnicalManagerScopeDepartments
                .ExistsAsync(
                    relation.TechnicalManagerScopeId,
                    relation.DepartmentId);

        if (exists &&
            (existing.TechnicalManagerScopeId !=
             relation.TechnicalManagerScopeId ||
             existing.DepartmentId != relation.DepartmentId))
        {
            throw new InvalidOperationException(
                "This department is already assigned to this technical manager scope.");
        }

        var existingRelations =
            await _unitOfWork.TechnicalManagerScopeDepartments
                .GetByScopeIdAsync(
                    relation.TechnicalManagerScopeId);

        if (relation.IsPrimary)
        {
            foreach (var item in existingRelations.Where(x =>
                x.Id != relation.Id))
            {
                item.IsPrimary = false;
                _unitOfWork.TechnicalManagerScopeDepartments
                    .Update(item);
            }
        }

        existing.TechnicalManagerScopeId =
            relation.TechnicalManagerScopeId;

        existing.DepartmentId =
            relation.DepartmentId;

        existing.Priority =
            relation.Priority;

        existing.IsPrimary =
            relation.IsPrimary;

        existing.IsActive =
            relation.IsActive;

        existing.ModifiedAt =
            DateTimeOffset.UtcNow;

        _unitOfWork.TechnicalManagerScopeDepartments
            .Update(existing);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var relation =
            await _unitOfWork.TechnicalManagerScopeDepartments
                .GetByIdAsync(id);

        if (relation is null)
            return false;

        relation.IsDeleted = true;
        relation.IsActive = false;
        relation.ModifiedAt = DateTimeOffset.UtcNow;

        _unitOfWork.TechnicalManagerScopeDepartments
            .Update(relation);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> ActivateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var relation =
            await _unitOfWork.TechnicalManagerScopeDepartments
                .GetByIdAsync(id);

        if (relation is null)
            return false;

        relation.IsActive = true;
        relation.IsDeleted = false;
        relation.ModifiedAt = DateTimeOffset.UtcNow;

        _unitOfWork.TechnicalManagerScopeDepartments
            .Update(relation);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var relation =
            await _unitOfWork.TechnicalManagerScopeDepartments
                .GetByIdAsync(id);

        if (relation is null)
            return false;

        relation.IsActive = false;
        relation.ModifiedAt = DateTimeOffset.UtcNow;

        _unitOfWork.TechnicalManagerScopeDepartments
            .Update(relation);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
