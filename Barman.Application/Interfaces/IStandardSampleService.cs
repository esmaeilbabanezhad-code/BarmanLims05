using Barman.Application.DTOs.StandardSample;
using Barman.Domain.Entities;

namespace Barman.Application.Interfaces;

public interface IStandardSampleService
{
    Task<List<StandardSample>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<StandardSample?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<List<StandardSample>> GetByMatrixIdAsync(
        Guid matrixId,
        CancellationToken cancellationToken = default);

    Task<StandardSample> CreateAsync(
        CreateStandardSampleDto dto,
        CancellationToken cancellationToken = default);

    Task<StandardSample> UpdateAsync(
        Guid id,
        UpdateStandardSampleDto dto,
        CancellationToken cancellationToken = default);

    Task<StandardSample> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<StandardSample> RestoreAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}