using Barman.Application.DTOs.LimitReference;
using Barman.Domain.Entities;

namespace Barman.Application.Interfaces;

public interface ILimitReferenceService
{
    Task<List<LimitReferenceLookupDto>> GetLookupAsync(
        CancellationToken cancellationToken = default);

    Task<LimitReferenceLookupDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<LimitReference?> FindByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<LimitReference> CreateAsync(
        CreateLimitReferenceDto dto,
        CancellationToken cancellationToken = default);

    Task<LimitReference> UpdateAsync(
        Guid id,
        UpdateLimitReferenceDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<List<LimitReference>> GetDeletedAsync(
        CancellationToken cancellationToken = default);

    Task<LimitReference> RestoreAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
