using Barman.Application.DTOs.LimitReference;
using Barman.Application.Interfaces;
using Barman.Domain.Entities;

namespace Barman.Application.Services;

public class LimitReferenceService : ILimitReferenceService
{
    private readonly IUnitOfWork _unitOfWork;

    public LimitReferenceService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<LimitReferenceLookupDto>> GetLookupAsync(
        CancellationToken cancellationToken = default)
    {
        var data = await _unitOfWork.LimitReferences
            .GetAllAsync(cancellationToken);

        return data
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Priority)
            .ThenBy(x => x.Name)
            .Select(x => new LimitReferenceLookupDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Version = x.Version,
                IssueDate = x.IssueDate,
                ValidFrom = x.ValidFrom,
                ValidTo = x.ValidTo,
                IsActive = x.IsActive,
                DocumentNo = x.DocumentNo,
                Priority = x.Priority,
                Description = x.Description
            })
            .ToList();
    }

    public async Task<LimitReferenceLookupDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.LimitReferences
            .GetByIdAsync(id, cancellationToken);

        if (entity == null)
            return null;

        return new LimitReferenceLookupDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            Version = entity.Version,
            IssueDate = entity.IssueDate,
            ValidFrom = entity.ValidFrom,
            ValidTo = entity.ValidTo,
            IsActive = entity.IsActive,
            DocumentNo = entity.DocumentNo,
            Priority = entity.Priority,
            Description = entity.Description
        };
    }

    public async Task<LimitReference?> FindByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.LimitReferences
            .GetByCodeAsync(code.Trim(), cancellationToken);
    }

    public async Task<LimitReference> CreateAsync(
        CreateLimitReferenceDto dto,
        CancellationToken cancellationToken = default)
    {
        var code = dto.Code.Trim();

        var existing = await _unitOfWork.LimitReferences
            .GetByCodeAsync(code, cancellationToken);

        if (existing != null && existing.IsDeleted)
        {
            existing.IsDeleted = false;
            existing.IsActive = true;
            existing.Name = dto.Name.Trim();
            existing.Version = dto.Version;
            existing.IssueDate = dto.IssueDate;
            existing.ValidFrom = dto.ValidFrom;
            existing.ValidTo = dto.ValidTo;
            existing.DocumentNo = dto.DocumentNo;
            existing.Priority = dto.Priority;
            existing.Description = dto.Description;

            _unitOfWork.LimitReferences.Update(existing);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return existing;
        }

        if (existing != null)
        {
            throw new InvalidOperationException(
                $"A limit reference with code '{code}' already exists.");
        }

        var entity = new LimitReference
        {
            Code = code,
            Name = dto.Name.Trim(),
            Version = dto.Version,
            IssueDate = dto.IssueDate,
            ValidFrom = dto.ValidFrom,
            ValidTo = dto.ValidTo,
            IsActive = true,
            DocumentNo = dto.DocumentNo,
            Priority = dto.Priority,
            Description = dto.Description,
            IsDeleted = false
        };

        await _unitOfWork.LimitReferences
            .AddAsync(entity);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<LimitReference> UpdateAsync(
        Guid id,
        UpdateLimitReferenceDto dto,
        CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.LimitReferences
            .GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException(
                "LimitReference not found.");

        entity.Code = dto.Code.Trim();
        entity.Name = dto.Name.Trim();
        entity.Version = dto.Version;
        entity.IssueDate = dto.IssueDate;
        entity.ValidFrom = dto.ValidFrom;
        entity.ValidTo = dto.ValidTo;
        entity.IsActive = dto.IsActive;
        entity.DocumentNo = dto.DocumentNo;
        entity.Priority = dto.Priority;
        entity.Description = dto.Description;

        _unitOfWork.LimitReferences.Update(entity);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.LimitReferences
            .GetByIdAsync(id, cancellationToken);

        if (entity == null)
            return false;

        entity.IsDeleted = true;
        entity.IsActive = false;

        _unitOfWork.LimitReferences.Update(entity);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<List<LimitReference>> GetDeletedAsync(
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.LimitReferences
            .GetDeletedAsync(cancellationToken);
    }

    public async Task<LimitReference> RestoreAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.LimitReferences
            .GetDeletedByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException(
                "LimitReference not found.");

        entity.IsDeleted = false;
        entity.IsActive = true;

        _unitOfWork.LimitReferences.Update(entity);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return entity;
    }
}
