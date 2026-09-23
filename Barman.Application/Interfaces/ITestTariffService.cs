using Barman.Domain.Entities;

namespace Barman.Application.Interfaces;

public interface ITestTariffService
{
    Task<List<TestTariff>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<TestTariff?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<List<TestTariff>> GetByTestIdAsync(
        Guid testId,
        CancellationToken cancellationToken = default);

    Task<List<TestTariff>> GetByTestPanelIdAsync(
        Guid testPanelId,
        CancellationToken cancellationToken = default);

    Task<List<TestTariff>> GetByOrganizationTypeIdAsync(
        Guid organizationTypeId,
        CancellationToken cancellationToken = default);

    Task<TestTariff?> GetApplicableAsync(
    Guid organizationTypeId,
    Guid? customerId,
    Guid? standardSampleId,
    Guid? matrixId,
    Guid? testId,
    Guid? testPanelId,
    DateTime effectiveDate,
    CancellationToken cancellationToken = default);

    Task<TestTariff> CreateAsync(
        TestTariff tariff,
        CancellationToken cancellationToken = default);

    Task<TestTariff> UpdateAsync(
        Guid id,
        TestTariff tariff,
        CancellationToken cancellationToken = default);

    Task<TestTariff> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<List<TestTariff>> GetForBulkAdjustmentAsync(
    Guid organizationTypeId,
    Guid? customerId,
    DateTime effectiveDate,
    List<Guid>? testIds,
    List<Guid>? testPanelIds,
    bool applyToAll);

    Task<int> ApplyBulkAdjustmentAsync(
        Barman.Application.DTOs.TestTariff.TariffBulkAdjustmentDto dto);

    Task<List<Barman.Application.DTOs.TestTariff.TariffBulkAdjustmentPreviewDto>>
    PreviewBulkAdjustmentAsync(
        Barman.Application.DTOs.TestTariff.TariffBulkAdjustmentDto dto);
}