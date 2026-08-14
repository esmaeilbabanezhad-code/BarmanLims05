using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface ITestPanelItemRepository
{
    Task<List<TestPanelItem>> GetByPanelIdAsync(
        Guid testPanelId,
        CancellationToken cancellationToken = default);

    Task<TestPanelItem?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        TestPanelItem entity,
        CancellationToken cancellationToken = default);

    void Update(TestPanelItem entity);

    void Delete(TestPanelItem entity);
}
