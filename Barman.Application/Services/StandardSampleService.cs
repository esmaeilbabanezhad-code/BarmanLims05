
using Barman.Application.DTOs.StandardSample;
using Barman.Application.Interfaces;
using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;


namespace Barman.Application.Services;

public class StandardSampleService : IStandardSampleService
{
    private readonly IStandardSampleRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public StandardSampleService(
        IStandardSampleRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<StandardSample>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }

    public async Task<StandardSample?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<List<StandardSample>> GetByMatrixIdAsync(
        Guid matrixId,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByMatrixIdAsync(
            matrixId,
            cancellationToken);
    }

    public async Task<StandardSample> CreateAsync(
        CreateStandardSampleDto dto,
        CancellationToken cancellationToken = default)
    {
        var entity = new StandardSample
        {
            Id = Guid.NewGuid(),
            Code = dto.Code.Trim(),
            Name = dto.Name.Trim(),
            SampleCategoryId = dto.SampleCategoryId,
            MatrixId = dto.MatrixId,
            Description = string.IsNullOrWhiteSpace(dto.Description)
                ? null
                : dto.Description.Trim(),
            IsActive = true,
            IsDeleted = false,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _repository.AddAsync(entity, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<StandardSample> UpdateAsync(
        Guid id,
        UpdateStandardSampleDto dto,
        CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(
            id,
            cancellationToken);

        if (entity == null)
            throw new InvalidOperationException(
                "Standard Sample یافت نشد.");

        entity.Code = dto.Code.Trim();
        entity.Name = dto.Name.Trim();
        entity.SampleCategoryId = dto.SampleCategoryId;
        entity.MatrixId = dto.MatrixId;
        entity.Description = string.IsNullOrWhiteSpace(dto.Description)
            ? null
            : dto.Description.Trim();

        entity.ModifiedAt = DateTimeOffset.UtcNow;

        _repository.Update(entity);

        return entity;
    }

    public async Task<StandardSample> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(
            id,
            cancellationToken);

        if (entity == null)
            throw new InvalidOperationException(
                "Standard Sample یافت نشد.");

        entity.IsDeleted = true;
        entity.IsActive = false;
        entity.ModifiedAt = DateTimeOffset.UtcNow;

        _repository.Update(entity);

        return entity;
    }

    public async Task<StandardSample> RestoreAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetDeletedByIdAsync(
            id,
            cancellationToken);

        if (entity == null)
            throw new InvalidOperationException(
                "Standard Sample حذف‌شده یافت نشد.");

        entity.IsDeleted = false;
        entity.IsActive = true;
        entity.ModifiedAt = DateTimeOffset.UtcNow;

        _repository.Update(entity);

        return entity;
    }
}