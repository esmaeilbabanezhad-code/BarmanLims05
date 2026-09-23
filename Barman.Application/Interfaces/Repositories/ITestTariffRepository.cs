using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface ITestTariffRepository
{
    Task<TestTariff?> GetByIdAsync(Guid id);

    Task<List<TestTariff>> GetAllAsync();

    Task<List<TestTariff>> GetByTestIdAsync(Guid testId);

    Task<List<TestTariff>> GetByTestPanelIdAsync(Guid testPanelId);

    Task<List<TestTariff>> GetByOrganizationTypeIdAsync(
        Guid organizationTypeId);

    Task<TestTariff?> GetApplicableAsync(
    Guid organizationTypeId,
    Guid? customerId,
    Guid? standardSampleId,
    Guid? matrixId,
    Guid? testId,
    Guid? testPanelId,
    DateTime effectiveDate);

    Task AddAsync(TestTariff tariff);

    void Update(TestTariff tariff);

    void Delete(TestTariff tariff);

    Task<List<TestTariff>> GetForBulkAdjustmentAsync(
    Guid organizationTypeId,
    Guid? customerId,
    DateTime effectiveDate,
    List<Guid>? testIds,
    List<Guid>? testPanelIds,
    bool applyToAll);

    Task BulkAdjustAsync(
        List<TestTariff> tariffsToClose,
        List<TestTariff> newTariffs);
}