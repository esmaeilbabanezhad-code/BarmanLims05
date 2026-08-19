using Barman.Application.Interfaces;
using Barman.Domain.Entities;

namespace Barman.Application.Services;

public class TechnicalManagerSectionHeadService
    : ITechnicalManagerSectionHeadService
{
    private readonly IUnitOfWork _unitOfWork;

    public TechnicalManagerSectionHeadService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TechnicalManagerSectionHead>>
        GetByTechnicalManagerIdAsync(
            Guid technicalManagerId,
            CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.TechnicalManagerSectionHeads
            .GetByTechnicalManagerIdAsync(technicalManagerId);
    }

    public async Task<List<TechnicalManagerSectionHead>>
        GetBySectionHeadIdAsync(
            Guid sectionHeadId,
            CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.TechnicalManagerSectionHeads
            .GetBySectionHeadIdAsync(sectionHeadId);
    }

    public async Task<TechnicalManagerSectionHead?>
        GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.TechnicalManagerSectionHeads
            .GetByIdAsync(id);
    }

    public async Task<TechnicalManagerSectionHead> AddAsync(
        TechnicalManagerSectionHead relation,
        CancellationToken cancellationToken = default)
    {
        var technicalManagerResponsibilities =
            await _unitOfWork.DepartmentResponsibilities
                .GetByEmployeeIdAsync(
                    relation.TechnicalManagerId);

        var isTechnicalManager =
            technicalManagerResponsibilities.Any(x =>
                x.ResponsibilityType ==
                    DepartmentResponsibilityType.TechnicalManager &&
                x.IsActive &&
                !x.IsDeleted);

        if (!isTechnicalManager)
        {
            throw new InvalidOperationException(
                "The selected employee is not an active Technical Manager.");
        }

        var sectionHeadResponsibilities =
            await _unitOfWork.DepartmentResponsibilities
                .GetByEmployeeIdAsync(
                    relation.SectionHeadId);

        var isSectionHead =
            sectionHeadResponsibilities.Any(x =>
                x.ResponsibilityType ==
                    DepartmentResponsibilityType.SectionHead &&
                x.IsActive &&
                !x.IsDeleted);

        if (!isSectionHead)
        {
            throw new InvalidOperationException(
                "The selected employee is not an active Section Head.");
        }

        var existing =
            await _unitOfWork.TechnicalManagerSectionHeads
                .GetByTechnicalManagerIdAsync(
                    relation.TechnicalManagerId);

        if (existing.Any(x =>
            x.SectionHeadId == relation.SectionHeadId))
        {
            throw new InvalidOperationException(
                "This Section Head is already assigned to this Technical Manager.");
        }

        relation.IsActive = true;
        relation.IsDeleted = false;

        await _unitOfWork.TechnicalManagerSectionHeads
            .AddAsync(relation);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return relation;
    }

    public async Task<bool> UpdateAsync(
        TechnicalManagerSectionHead relation,
        CancellationToken cancellationToken = default)
    {
        var existing =
            await _unitOfWork.TechnicalManagerSectionHeads
                .GetByIdAsync(relation.Id);

        if (existing is null)
            return false;

        var technicalManagerResponsibilities =
            await _unitOfWork.DepartmentResponsibilities
                .GetByEmployeeIdAsync(
                    relation.TechnicalManagerId);

        var isTechnicalManager =
            technicalManagerResponsibilities.Any(x =>
                x.ResponsibilityType ==
                    DepartmentResponsibilityType.TechnicalManager &&
                x.IsActive &&
                !x.IsDeleted);

        if (!isTechnicalManager)
        {
            throw new InvalidOperationException(
                "The selected employee is not an active Technical Manager.");
        }

        var sectionHeadResponsibilities =
            await _unitOfWork.DepartmentResponsibilities
                .GetByEmployeeIdAsync(
                    relation.SectionHeadId);

        var isSectionHead =
            sectionHeadResponsibilities.Any(x =>
                x.ResponsibilityType ==
                    DepartmentResponsibilityType.SectionHead &&
                x.IsActive &&
                !x.IsDeleted);

        if (!isSectionHead)
        {
            throw new InvalidOperationException(
                "The selected employee is not an active Section Head.");
        }

        var duplicate =
            await _unitOfWork.TechnicalManagerSectionHeads
                .GetByTechnicalManagerIdAsync(
                    relation.TechnicalManagerId);

        if (duplicate.Any(x =>
            x.Id != relation.Id &&
            x.SectionHeadId == relation.SectionHeadId))
        {
            throw new InvalidOperationException(
                "This Section Head is already assigned to this Technical Manager.");
        }

        existing.TechnicalManagerId = relation.TechnicalManagerId;
        existing.SectionHeadId = relation.SectionHeadId;
        existing.IsActive = relation.IsActive;

        existing.ModifiedAt = DateTimeOffset.UtcNow;

        _unitOfWork.TechnicalManagerSectionHeads
            .Update(existing);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var relation =
            await _unitOfWork.TechnicalManagerSectionHeads
                .GetByIdAsync(id);

        if (relation is null)
            return false;

        relation.IsDeleted = true;
        relation.IsActive = false;
        relation.ModifiedAt = DateTimeOffset.UtcNow;

        _unitOfWork.TechnicalManagerSectionHeads
            .Update(relation);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> ActivateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var relation =
            await _unitOfWork.TechnicalManagerSectionHeads
                .GetByIdAsync(id);

        if (relation is null)
            return false;

        relation.IsActive = true;
        relation.IsDeleted = false;
        relation.ModifiedAt = DateTimeOffset.UtcNow;

        _unitOfWork.TechnicalManagerSectionHeads
            .Update(relation);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var relation =
            await _unitOfWork.TechnicalManagerSectionHeads
                .GetByIdAsync(id);

        if (relation is null)
            return false;

        relation.IsActive = false;
        relation.ModifiedAt = DateTimeOffset.UtcNow;

        _unitOfWork.TechnicalManagerSectionHeads
            .Update(relation);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
