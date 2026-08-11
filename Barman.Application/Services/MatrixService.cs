using Barman.Application.DTOs.Matrix;
using Barman.Application.Interfaces;
using Barman.Domain.Entities;

namespace Barman.Application.Services;

public class MatrixService : IMatrixService
{
    private readonly IUnitOfWork _unitOfWork;

    public MatrixService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<Matrix>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Matrices
            .GetAllAsync(cancellationToken);
    }

    public async Task<Matrix?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return null;

        return await _unitOfWork.Matrices
            .GetByIdAsync(id, cancellationToken);
    }

    public async Task<List<Matrix>> GetBySampleCategoryIdAsync(
        Guid sampleCategoryId,
        CancellationToken cancellationToken = default)
    {
        if (sampleCategoryId == Guid.Empty)
            return new List<Matrix>();

        return await _unitOfWork.Matrices
            .GetBySampleCategoryIdAsync(
                sampleCategoryId,
                cancellationToken);
    }

    public async Task<List<Matrix>> GetDeletedAsync(
    CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Matrices
            .GetDeletedAsync(cancellationToken);
    }

    public async Task<Matrix?> GetDeletedByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return null;

        return await _unitOfWork.Matrices
            .GetDeletedByIdAsync(id, cancellationToken);
    }

    public async Task<Matrix> CreateAsync(
        CreateMatrixDto dto,
        CancellationToken cancellationToken = default)
    {
        var matrix = new Matrix
        {
            Id = Guid.NewGuid(),
            Code = dto.Code.Trim().ToUpperInvariant(),
            Name = dto.Name.Trim(),
            Description = dto.Description,
            SampleCategoryId = dto.SampleCategoryId,
            IsActive = true,
            IsDeleted = false
        };

        await _unitOfWork.Matrices
            .AddAsync(matrix, cancellationToken);

        await _unitOfWork
            .SaveChangesAsync(cancellationToken);

        return matrix;
    }

    public async Task<Matrix> UpdateAsync(
        Guid id,
        UpdateMatrixDto dto,
        CancellationToken cancellationToken = default)
    {
        var matrix =
            await _unitOfWork.Matrices
                .GetByIdAsync(id, cancellationToken);

        if (matrix == null)
            throw new KeyNotFoundException(
                "Matrix not found.");

        matrix.Code =
            dto.Code.Trim().ToUpperInvariant();

        matrix.Name =
            dto.Name.Trim();

        matrix.Description =
            dto.Description;

        matrix.SampleCategoryId =
            dto.SampleCategoryId;

        matrix.IsActive =
            dto.IsActive;

        await _unitOfWork
            .SaveChangesAsync(cancellationToken);

        return matrix;
    }

    public async Task<Matrix> ActivateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var matrix =
            await _unitOfWork.Matrices
                .GetByIdAsync(id, cancellationToken);

        if (matrix == null)
            throw new KeyNotFoundException();

        matrix.IsActive = true;

        await _unitOfWork
            .SaveChangesAsync(cancellationToken);

        return matrix;
    }

    public async Task<Matrix> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var matrix =
            await _unitOfWork.Matrices
                .GetByIdAsync(id, cancellationToken);

        if (matrix == null)
            throw new KeyNotFoundException();

        matrix.IsActive = false;

        await _unitOfWork
            .SaveChangesAsync(cancellationToken);

        return matrix;
    }

    public async Task<Matrix> DeleteAsync(
    Guid id,
    CancellationToken cancellationToken = default)
    {
        var matrix =
            await _unitOfWork.Matrices
                .GetByIdAsync(id, cancellationToken);

        if (matrix == null)
            throw new KeyNotFoundException(
                "Matrix not found.");

        matrix.IsDeleted = true;
        matrix.IsActive = false;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return matrix;
    }

    public async Task<Matrix> RestoreAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var matrix =
            await _unitOfWork.Matrices
                .GetDeletedByIdAsync(id, cancellationToken);

        if (matrix == null)
            throw new KeyNotFoundException(
                "Deleted matrix not found.");

        matrix.IsDeleted = false;
        matrix.IsActive = true;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return matrix;
    }
}