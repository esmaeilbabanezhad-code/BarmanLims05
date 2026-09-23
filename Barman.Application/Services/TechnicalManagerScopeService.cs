using Barman.Application.Interfaces;
using Barman.Domain.Entities;

namespace Barman.Application.Services;

public class TechnicalManagerScopeService
    : ITechnicalManagerScopeService
{
    private readonly IUnitOfWork _unitOfWork;

    public TechnicalManagerScopeService(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TechnicalManagerScope>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.TechnicalManagerScopes
            .GetAllAsync(cancellationToken);
    }

    public async Task<TechnicalManagerScope?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.TechnicalManagerScopes
            .GetByIdAsync(id, cancellationToken);
    }

    public async Task<TechnicalManagerScope?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.TechnicalManagerScopes
            .GetByCodeAsync(code, cancellationToken);
    }

    public async Task<TechnicalManagerScope> AddAsync(
        TechnicalManagerScope scope,
        CancellationToken cancellationToken = default)
    {
        if (scope == null)
            throw new ArgumentNullException(nameof(scope));

        scope.Code = scope.Code?.Trim() ?? "";
        scope.Name = scope.Name?.Trim() ?? "";
        scope.Description = string.IsNullOrWhiteSpace(scope.Description)
            ? null
            : scope.Description.Trim();

        if (string.IsNullOrWhiteSpace(scope.Code))
            throw new InvalidOperationException(
                "کد محدوده مسئول فنی الزامی است.");

        if (string.IsNullOrWhiteSpace(scope.Name))
            throw new InvalidOperationException(
                "نام محدوده مسئول فنی الزامی است.");

        if (scope.Priority < 0)
            throw new InvalidOperationException(
                "اولویت نمی‌تواند منفی باشد.");

        var existing =
            await _unitOfWork.TechnicalManagerScopes
                .GetByCodeAsync(
                    scope.Code,
                    cancellationToken);

        if (existing != null)
            throw new InvalidOperationException(
                "کد محدوده مسئول فنی تکراری است.");

        scope.IsActive = true;
        scope.IsDeleted = false;

        await _unitOfWork.TechnicalManagerScopes
            .AddAsync(scope, cancellationToken);

        await _unitOfWork.SaveChangesAsync();

        return scope;
    }

    public async Task<bool> UpdateAsync(
        TechnicalManagerScope scope,
        CancellationToken cancellationToken = default)
    {
        if (scope == null)
            throw new ArgumentNullException(nameof(scope));

        if (scope.Id == Guid.Empty)
            throw new InvalidOperationException(
                "شناسه محدوده مسئول فنی نامعتبر است.");

        scope.Code = scope.Code?.Trim() ?? "";
        scope.Name = scope.Name?.Trim() ?? "";
        scope.Description = string.IsNullOrWhiteSpace(scope.Description)
            ? null
            : scope.Description.Trim();

        if (string.IsNullOrWhiteSpace(scope.Code))
            throw new InvalidOperationException(
                "کد محدوده مسئول فنی الزامی است.");

        if (string.IsNullOrWhiteSpace(scope.Name))
            throw new InvalidOperationException(
                "نام محدوده مسئول فنی الزامی است.");

        if (scope.Priority < 0)
            throw new InvalidOperationException(
                "اولویت نمی‌تواند منفی باشد.");

        var existing =
            await _unitOfWork.TechnicalManagerScopes
                .GetByIdAsync(
                    scope.Id,
                    cancellationToken);

        if (existing == null)
            return false;

        var duplicate =
            await _unitOfWork.TechnicalManagerScopes
                .GetByCodeAsync(
                    scope.Code,
                    cancellationToken);

        if (duplicate != null &&
            duplicate.Id != scope.Id)
        {
            throw new InvalidOperationException(
                "کد محدوده مسئول فنی تکراری است.");
        }

        existing.Code = scope.Code;
        existing.Name = scope.Name;
        existing.Description = scope.Description;
        existing.Priority = scope.Priority;
        existing.IsActive = scope.IsActive;

        existing.ModifiedAt = DateTimeOffset.UtcNow;

        _unitOfWork.TechnicalManagerScopes
            .Update(existing);

        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var scope =
            await _unitOfWork.TechnicalManagerScopes
                .GetByIdAsync(
                    id,
                    cancellationToken);

        if (scope == null)
            return false;

        scope.IsDeleted = true;
        scope.IsActive = false;
        scope.ModifiedAt = DateTimeOffset.UtcNow;

        _unitOfWork.TechnicalManagerScopes
            .Update(scope);

        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ActivateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var scope =
            await _unitOfWork.TechnicalManagerScopes
                .GetByIdAsync(
                    id,
                    cancellationToken);

        if (scope == null)
            return false;

        scope.IsDeleted = false;
        scope.IsActive = true;
        scope.ModifiedAt = DateTimeOffset.UtcNow;

        _unitOfWork.TechnicalManagerScopes
            .Update(scope);

        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var scope =
            await _unitOfWork.TechnicalManagerScopes
                .GetByIdAsync(
                    id,
                    cancellationToken);

        if (scope == null)
            return false;

        scope.IsActive = false;
        scope.ModifiedAt = DateTimeOffset.UtcNow;

        _unitOfWork.TechnicalManagerScopes
            .Update(scope);

        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
