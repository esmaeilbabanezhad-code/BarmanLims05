using Barman.Application.DTOs.Matrix;
using Barman.Domain.Entities;

namespace Barman.Application.Interfaces;

public interface IMatrixService
{
    Task<List<Matrix>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Matrix?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<List<Matrix>> GetBySampleCategoryIdAsync(
        Guid sampleCategoryId,
        CancellationToken cancellationToken = default);

    Task<Matrix> DeleteAsync(
    Guid id,
    CancellationToken cancellationToken = default);

    Task<Matrix> RestoreAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<List<Matrix>> GetDeletedAsync(
    CancellationToken cancellationToken = default);

    Task<Matrix?> GetDeletedByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Matrix> CreateAsync(
        CreateMatrixDto dto,
        CancellationToken cancellationToken = default);

    Task<Matrix> UpdateAsync(
        Guid id,
        UpdateMatrixDto dto,
        CancellationToken cancellationToken = default);

    Task<Matrix> ActivateAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Matrix> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}