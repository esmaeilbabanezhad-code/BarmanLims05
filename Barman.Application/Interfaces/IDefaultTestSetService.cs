using Barman.Application.DTOs.DefaultTestSet;

namespace Barman.Application.Interfaces;

public interface IDefaultTestSetService
{
    Task<List<DefaultTestSetDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<List<DefaultTestSetDto>> GetByCustomerAsync(
        Guid customerId,
        CancellationToken cancellationToken = default);

    Task<DefaultTestSetDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<DefaultTestSetDto?> ResolveAsync(
        Guid? customerId,
        Guid? sampleCategoryId,
        Guid? matrixId,
        CancellationToken cancellationToken = default);

    Task<DefaultTestSetDto> CreateAsync(
        DefaultTestSetDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        DefaultTestSetDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}