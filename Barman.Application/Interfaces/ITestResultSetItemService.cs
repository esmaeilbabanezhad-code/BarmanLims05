using Barman.Application.DTOs.Test;

namespace Barman.Application.Interfaces;

public interface ITestResultSetItemService
{
    Task<List<TestResultSetItemDto>> GetByResultSetIdAsync(
        Guid testResultSetId,
        CancellationToken cancellationToken = default);

    Task<TestResultSetItemDto> AddAsync(
        Guid testResultSetId,
        Guid testResultDefinitionId,
        int displayOrder,
        decimal? lod,
        decimal? loq,
        decimal? minValue,
        decimal? maxValue,
        CancellationToken cancellationToken = default);

    Task<TestResultSetItemDto> UpdateAsync(
        Guid id,
        int displayOrder,
        decimal? lod,
        decimal? loq,
        decimal? minValue,
        decimal? maxValue,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateDisplayOrderAsync(
        Guid id,
        int displayOrder,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}