using Barman.Application.Interfaces;
using Barman.Application.Interfaces.Repositories.Reporting;
using Barman.Application.Interfaces.Services.Reporting;
using Barman.Domain.Entities.Reporting;

namespace Barman.Application.Services.Reporting;

public class ReportTemplateFieldService : IReportTemplateFieldService
{
    private readonly IReportTemplateFieldRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ReportTemplateFieldService(
        IReportTemplateFieldRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<ReportTemplateField>> GetBySectionIdAsync(
        Guid sectionId,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetBySectionIdAsync(
            sectionId,
            cancellationToken);
    }

    public async Task<ReportTemplateField> CreateAsync(
        ReportTemplateField entity,
        CancellationToken cancellationToken = default)
    {
        await _repository.AddAsync(entity, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<bool> UpdateAsync(
        ReportTemplateField entity,
        CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(
            entity.Id,
            cancellationToken);

        if (existing is null)
            return false;

        existing.Source = entity.Source;
        existing.FieldCode = entity.FieldCode;
        existing.FieldType = entity.FieldType;
        existing.Caption = entity.Caption;
        existing.DisplayOrder = entity.DisplayOrder;
        existing.IsVisible = entity.IsVisible;
        existing.Width = entity.Width;
        existing.Alignment = entity.Alignment;
        existing.Format = entity.Format;
        existing.Description = entity.Description;

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