using Barman.Domain.Entities;

namespace Barman.Application.Interfaces;

public interface ITestPanelItemService
{
    Task<List<TestPanelItem>> GetByPanelIdAsync(
        Guid testPanelId,
        CancellationToken cancellationToken = default);

    Task<TestPanelItem> AddAsync(
        Guid testPanelId,
        Guid testId,
        int sortOrder,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateSortOrderAsync(
        Guid id,
        int sortOrder,
        CancellationToken cancellationToken = default);
}
