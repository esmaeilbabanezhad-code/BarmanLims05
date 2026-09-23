using Barman.Application.Interfaces;
using Barman.Domain.Entities;

namespace Barman.Application.Services;

public class OrganizationTypeService : IOrganizationTypeService
{
    private readonly IUnitOfWork _unitOfWork;

    public OrganizationTypeService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<OrganizationType>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.OrganizationTypes.GetAllAsync();
    }

    public async Task<OrganizationType?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return null;

        return await _unitOfWork.OrganizationTypes.GetByIdAsync(id);
    }
    public async Task<List<OrganizationType>> GetDeletedAsync(
    CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.OrganizationTypes
            .GetDeletedAsync();
    }

    public async Task<OrganizationType?> GetDeletedByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return null;

        return await _unitOfWork.OrganizationTypes
            .GetDeletedByIdAsync(id);
    }
    public async Task<OrganizationType> CreateAsync(
        OrganizationType organizationType,
        CancellationToken cancellationToken = default)
    {
        if (organizationType == null)
            throw new ArgumentNullException(nameof(organizationType));

        if (string.IsNullOrWhiteSpace(organizationType.Code))
            throw new ArgumentException("کد نوع سازمان الزامی است.");

        if (string.IsNullOrWhiteSpace(organizationType.Name))
            throw new ArgumentException("نام نوع سازمان الزامی است.");

        organizationType.Code =
            organizationType.Code.Trim().ToUpperInvariant();

        organizationType.Name =
            organizationType.Name.Trim();

        organizationType.IsDeleted = false;
        organizationType.IsActive = true;

        await _unitOfWork.OrganizationTypes.AddAsync(organizationType);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return organizationType;
    }

    public async Task<OrganizationType> UpdateAsync(
        Guid id,
        OrganizationType organizationType,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("شناسه نامعتبر است.");

        if (organizationType == null)
            throw new ArgumentNullException(nameof(organizationType));

        var existing =
            await _unitOfWork.OrganizationTypes.GetByIdAsync(id);

        if (existing == null)
            throw new KeyNotFoundException(
                "نوع سازمان یافت نشد.");

        if (string.IsNullOrWhiteSpace(organizationType.Code))
            throw new ArgumentException("کد نوع سازمان الزامی است.");

        if (string.IsNullOrWhiteSpace(organizationType.Name))
            throw new ArgumentException("نام نوع سازمان الزامی است.");

        existing.Code =
            organizationType.Code.Trim().ToUpperInvariant();

        existing.Name =
            organizationType.Name.Trim();

        existing.Description =
            organizationType.Description;

        existing.DisplayOrder =
            organizationType.DisplayOrder;

        existing.IsActive =
            organizationType.IsActive;

        _unitOfWork.OrganizationTypes.Update(existing);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return existing;
    }

    public async Task<OrganizationType> ActivateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var organizationType =
            await _unitOfWork.OrganizationTypes.GetByIdAsync(id);

        if (organizationType == null)
            throw new KeyNotFoundException(
                "نوع سازمان یافت نشد.");

        organizationType.IsActive = true;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return organizationType;
    }

    public async Task<OrganizationType> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var organizationType =
            await _unitOfWork.OrganizationTypes.GetByIdAsync(id);

        if (organizationType == null)
            throw new KeyNotFoundException(
                "نوع سازمان یافت نشد.");

        organizationType.IsActive = false;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return organizationType;
    }

    public async Task<OrganizationType> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var organizationType =
            await _unitOfWork.OrganizationTypes.GetByIdAsync(id);

        if (organizationType == null)
            throw new KeyNotFoundException(
                "نوع سازمان یافت نشد.");

        organizationType.IsDeleted = true;
        organizationType.IsActive = false;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return organizationType;
    }
    public async Task<OrganizationType> RestoreAsync(
    Guid id,
    CancellationToken cancellationToken = default)
    {
        var organizationType =
            await _unitOfWork.OrganizationTypes
                .GetDeletedByIdAsync(id);

        if (organizationType == null)
            throw new KeyNotFoundException(
                "نوع سازمان حذف‌شده یافت نشد.");

        organizationType.IsDeleted = false;
        organizationType.IsActive = true;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return organizationType;
    }
}