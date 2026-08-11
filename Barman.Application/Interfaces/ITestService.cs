namespace Barman.Application.Interfaces;

using Barman.Application.DTOs.Test;
using Barman.Domain.Entities;

public interface ITestService
{
    Task<List<TestLookupDto>> GetReceptionLookupAsync(
        CancellationToken cancellationToken = default);

    Task<List<TestLookupDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<TestLookupDto> ActivateAsync(
    Guid id,
    CancellationToken cancellationToken = default);

    Task<TestLookupDto> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<List<TestLookupDto>> GetDeletedAsync(
    CancellationToken cancellationToken = default);

    Task<TestLookupDto?> RestoreAsync(
    Guid id,
    CancellationToken cancellationToken = default);

    Task<TestLookupDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<TestLookupDto> CreateAsync(
        CreateTestDto dto,
        CancellationToken cancellationToken = default);

    Task<TestLookupDto?> UpdateAsync(
        Guid id,
        UpdateTestDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<List<Department>> GetDepartmentsAsync(
    CancellationToken cancellationToken = default);

    Task<List<SampleCategory>> GetSampleCategoriesAsync(
        CancellationToken cancellationToken = default);

    Task<List<Matrix>> GetMatricesAsync(
        Guid? sampleCategoryId = null,
        CancellationToken cancellationToken = default);

}