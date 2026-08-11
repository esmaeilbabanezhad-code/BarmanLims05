using Barman.Application.DTOs.SampleCategory;
using Barman.Application.Interfaces;
using Barman.Domain.Entities;

namespace Barman.Application.Services;

public class SampleCategoryService : ISampleCategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public SampleCategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<SampleCategory>> GetAllAsync()
    {
        return await _unitOfWork.SampleCategories.GetAllAsync();
    }

    public async Task<List<SampleCategoryLookupDto>> GetLookupAsync(
        CancellationToken cancellationToken = default)
    {
        var data =
            await _unitOfWork.SampleCategories
                .GetAllAsync(cancellationToken);

        return data
            .Where(x => !x.IsDeleted && x.IsActive)
            .OrderBy(x => x.Name)
            .Select(x => new SampleCategoryLookupDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description
            })
            .ToList();
    }

    public async Task<SampleCategoryLookupDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var entity =
            await _unitOfWork.SampleCategories
                .GetByIdAsync(id, cancellationToken);

        if (entity == null)
            return null;

        return new SampleCategoryLookupDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            Description = entity.Description
        };
    }

    public async Task<SampleCategory?> FindByCodeOrNameAsync(
        string code,
        string name,
        CancellationToken cancellationToken = default)
    {
        var list =
            await _unitOfWork.SampleCategories
                .GetAllAsync(cancellationToken);

        return list.FirstOrDefault(x =>
            !x.IsDeleted &&
            (x.Code == code || x.Name == name));
    }

    public async Task<SampleCategory> CreateAsync(
    CreateSampleCategoryDto dto,
    CancellationToken cancellationToken = default)
    {
        var code = dto.Code.Trim();

        var existing =
            await _unitOfWork.SampleCategories
                .GetByCodeAsync(code, cancellationToken);

        // اگر قبلاً حذف شده، همان رکورد را بازیابی کن
        if (existing != null && existing.IsDeleted)
        {
            existing.IsDeleted = false;
            existing.IsActive = true;
            existing.Name = dto.Name.Trim();
            existing.Description = dto.Description;

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            return existing;
        }

        // اگر رکورد فعال یا غیرفعال موجود است، رکورد تکراری نساز
        if (existing != null)
        {
            throw new InvalidOperationException(
                $"A sample category with code '{code}' already exists.");
        }

        var entity = new SampleCategory
        {
            Code = code,
            Name = dto.Name.Trim(),
            Description = dto.Description,
            IsActive = true,
            IsDeleted = false
        };

        await _unitOfWork.SampleCategories
            .AddAsync(entity);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return entity;
    }

    public async Task<SampleCategory> UpdateAsync(
        Guid id,
        UpdateSampleCategoryDto dto,
        CancellationToken cancellationToken = default)
    {
        var entity =
            await _unitOfWork.SampleCategories
                .GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException(
                "SampleCategory not found.");

        entity.Code =
            dto.Code.Trim().ToUpperInvariant();

        entity.Name =
            dto.Name.Trim();

        entity.Description =
            dto.Description;

        entity.IsActive =
            dto.IsActive;

        _unitOfWork.SampleCategories.Update(entity);

        await _unitOfWork
            .SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<SampleCategory> ActivateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var entity =
            await _unitOfWork.SampleCategories
                .GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException(
                "SampleCategory not found.");

        entity.IsActive = true;

        _unitOfWork.SampleCategories.Update(entity);

        await _unitOfWork
            .SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<SampleCategory> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var entity =
            await _unitOfWork.SampleCategories
                .GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException(
                "SampleCategory not found.");

        entity.IsActive = false;

        _unitOfWork.SampleCategories.Update(entity);

        await _unitOfWork
            .SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<bool> DeleteAsync(
    Guid id,
    CancellationToken cancellationToken = default)
    {
        var category =
            await _unitOfWork.SampleCategories
                .GetByIdAsync(id, cancellationToken);

        if (category == null)
            return false;

        category.IsDeleted = true;
        category.IsActive = false;

        _unitOfWork.SampleCategories.Update(category);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<List<SampleCategory>> GetDeletedAsync(
    CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.SampleCategories
            .GetDeletedAsync(cancellationToken);
    }

    public async Task<SampleCategory> RestoreAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var category =
            await _unitOfWork.SampleCategories
                .GetDeletedByIdAsync(id, cancellationToken);

        if (category == null)
            throw new KeyNotFoundException(
                "Sample Category not found.");

        category.IsDeleted = false;
        category.IsActive = true;

        _unitOfWork.SampleCategories.Update(category);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return category;
    }

}