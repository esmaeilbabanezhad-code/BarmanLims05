using Barman.Application.Interfaces.Repositories.Reporting;
using Barman.Application.Interfaces.Services.Reporting;
using Barman.Domain.Entities.Reporting;
using Barman.Application.Interfaces;

namespace Barman.Application.Services.Reporting;

public class ReportTemplateSectionService : IReportTemplateSectionService
{
    private readonly IReportTemplateSectionRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ReportTemplateSectionService(
        IReportTemplateSectionRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<ReportTemplateSection>> GetByTemplateIdAsync(
        Guid templateId,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByTemplateIdAsync(
            templateId,
            cancellationToken);
    }

    public async Task<ReportTemplateSection> CreateAsync(
        ReportTemplateSection entity,
        CancellationToken cancellationToken = default)
    {
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> UpdateAsync(
        ReportTemplateSection entity,
        CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(
            entity.Id,
            cancellationToken);

        if (existing is null)
            return false;

        existing.Code = entity.Code;
        existing.Name = entity.Name;
        existing.DisplayOrder = entity.DisplayOrder;
        existing.IsVisible = entity.IsVisible;

        _repository.Update(existing);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(
            id,
            cancellationToken);

        if (existing is null)
            return false;

        _repository.Delete(existing);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
