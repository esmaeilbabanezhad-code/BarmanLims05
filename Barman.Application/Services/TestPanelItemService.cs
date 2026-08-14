using Barman.Application.Interfaces;
using Barman.Domain.Entities;

namespace Barman.Application.Services;

public class TestPanelItemService : ITestPanelItemService
{
    private readonly IUnitOfWork _unitOfWork;

    public TestPanelItemService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TestPanelItem>> GetByPanelIdAsync(
        Guid testPanelId,
        CancellationToken cancellationToken = default)
    {
        if (testPanelId == Guid.Empty)
            return new List<TestPanelItem>();

        return await _unitOfWork.TestPanelItems
            .GetByPanelIdAsync(
                testPanelId,
                cancellationToken);
    }

    public async Task<TestPanelItem> AddAsync(
        Guid testPanelId,
        Guid testId,
        int sortOrder,
        CancellationToken cancellationToken = default)
    {
        if (testPanelId == Guid.Empty)
            throw new ArgumentException(
                "TestPanelId is required.");

        if (testId == Guid.Empty)
            throw new ArgumentException(
                "TestId is required.");

        if (sortOrder < 0)
            throw new ArgumentException(
                "SortOrder cannot be negative.");

        var panel = await _unitOfWork.TestPanels
            .GetByIdAsync(
                testPanelId,
                cancellationToken);

        if (panel == null)
            throw new KeyNotFoundException(
                "Test Panel not found.");

        var test = await _unitOfWork.Tests
            .GetByIdAsync(testId);

        if (test == null || test.IsDeleted)
            throw new KeyNotFoundException(
                "Test not found.");

        var existingItems =
            await _unitOfWork.TestPanelItems
                .GetByPanelIdAsync(
                    testPanelId,
                    cancellationToken);

        if (existingItems.Any(x => x.TestId == testId))
        {
            throw new InvalidOperationException(
                "??? ????? ????? ?? ??? Panel ????? ??? ???.");
        }

        var item = new TestPanelItem
        {
            Id = Guid.NewGuid(),
            TestPanelId = testPanelId,
            TestId = testId,
            SortOrder = sortOrder
        };

        await _unitOfWork.TestPanelItems
            .AddAsync(
                item,
                cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return item;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return false;

        var item = await FindItemAsync(
            id,
            cancellationToken);

        if (item == null)
            return false;

        _unitOfWork.TestPanelItems.Delete(item);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool> UpdateSortOrderAsync(
        Guid id,
        int sortOrder,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return false;

        if (sortOrder < 0)
            throw new ArgumentException(
                "SortOrder cannot be negative.");

        var item = await FindItemAsync(
            id,
            cancellationToken);

        if (item == null)
            return false;

        item.SortOrder = sortOrder;

        _unitOfWork.TestPanelItems.Update(item);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    private async Task<TestPanelItem?> FindItemAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _unitOfWork.TestPanelItems
            .GetByIdAsync(
                id,
                cancellationToken);
    }
}
