using Barman.Application.DTOs.Test;

namespace Barman.Application.Interfaces;

public interface ITestResultSetService
{
    Task<List<TestResultSetDto>> GetByTestIdAsync(
        Guid testId,
        CancellationToken cancellationToken = default);

    Task<TestResultSetDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<TestResultSetDto> CreateAsync(
        TestResultSetDto dto,
        CancellationToken cancellationToken = default);

    Task<TestResultSetDto> UpdateAsync(
        TestResultSetDto dto,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}