using Barman.Application.Interfaces.Repositories.Reporting;
using Barman.Application.Interfaces.Services.Reporting;
using Barman.Domain.Entities.Reporting;
using Barman.Application.Interfaces;

namespace Barman.Application.Services.Reporting;

public class ReportTemplateService : IReportTemplateService
{
    private readonly IReportTemplateRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ReportTemplateService(
        IReportTemplateRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public Task<List<ReportTemplate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return _repository.GetAllAsync(cancellationToken);
    }

    public Task<ReportTemplate?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdAsync(id, cancellationToken);
    }

    public Task<ReportTemplate?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetByCodeAsync(code, cancellationToken);
    }

    public async Task<ReportTemplate> CreateAsync(
        ReportTemplate entity,
        CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByCodeAsync(
            entity.Code,
            cancellationToken);

        if (existing is not null)
            throw new InvalidOperationException(
                $"Report template code '{entity.Code}' already exists.");

        await _repository.AddAsync(entity, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<bool> UpdateAsync(
    ReportTemplate entity,
    CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(
            entity.Id,
            cancellationToken);

        if (existing is null)
            return false;

        existing.Code = entity.Code;
        existing.Name = entity.Name;
        existing.ReportType = entity.ReportType;
        existing.Authority = entity.Authority;
        existing.Version = entity.Version;
        existing.IsActive = entity.IsActive;
        existing.Description = entity.Description;

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

    public async Task<ReportTemplate?> CloneAsync(
    Guid templateId,
    string newCode,
    string newName,
    CancellationToken cancellationToken = default)
    {
        var source = await _repository.GetWithSectionsAsync(
            templateId,
            cancellationToken);

        if (source is null)
            return null;

        var existingCode = await _repository.GetByCodeAsync(
            newCode,
            cancellationToken);

        if (existingCode is not null)
            throw new InvalidOperationException(
                $"قالبی با کد '{newCode}' قبلاً وجود دارد.");

        var clone = new ReportTemplate
        {
            Code = newCode.Trim(),
            Name = newName.Trim(),
            ReportType = source.ReportType,
            Authority = source.Authority,
            Version = source.Version,
            IsActive = true,
            IsSystemTemplate = false,
            IsSystemDefault = false,
            CanDelete = true,
            Description = source.Description
        };

        foreach (var sourceSection in source.Sections
                     .OrderBy(x => x.DisplayOrder))
        {
            var clonedSection = new ReportTemplateSection
            {
                Code = sourceSection.Code,
                Name = sourceSection.Name,
                DisplayOrder = sourceSection.DisplayOrder,
                IsVisible = sourceSection.IsVisible
            };

            foreach (var sourceField in sourceSection.Fields
                         .OrderBy(x => x.DisplayOrder))
            {
                clonedSection.Fields.Add(
                    new ReportTemplateField
                    {
                        Source = sourceField.Source,
                        FieldCode = sourceField.FieldCode,
                        FieldType = sourceField.FieldType,
                        Caption = sourceField.Caption,
                        DisplayOrder = sourceField.DisplayOrder,
                        IsVisible = sourceField.IsVisible,
                        Width = sourceField.Width,
                        Alignment = sourceField.Alignment,
                        Format = sourceField.Format,
                        Description = sourceField.Description
                    });
            }

            clone.Sections.Add(clonedSection);
        }

        await _repository.AddAsync(
            clone,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return clone;
    }

}
