using Barman.Application.DTOs.SampleCategory;
using Barman.Domain.Entities;

namespace Barman.Application.Interfaces;

public interface ISampleCategoryService
{
    Task<List<SampleCategoryLookupDto>> GetLookupAsync(
        CancellationToken cancellationToken = default);

    Task<SampleCategoryLookupDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<SampleCategory?> FindByCodeOrNameAsync(
        string code,
        string name,
        CancellationToken cancellationToken = default);

    Task<SampleCategory> CreateAsync(
        CreateSampleCategoryDto dto,
        CancellationToken cancellationToken = default);

    Task<SampleCategory> UpdateAsync(
        Guid id,
        UpdateSampleCategoryDto dto,
        CancellationToken cancellationToken = default);

    Task<SampleCategory> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<SampleCategory> ActivateAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    // برای ComboBox های LIMS
    Task<List<SampleCategory>> GetAllAsync();

    // حذف منطقی؛ رکورد فیزیکی حذف نمی‌شود
    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    Task<List<SampleCategory>> GetDeletedAsync(
    CancellationToken cancellationToken = default);

    Task<SampleCategory> RestoreAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}