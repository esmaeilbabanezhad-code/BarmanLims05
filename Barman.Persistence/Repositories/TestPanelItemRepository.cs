using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class TestPanelItemRepository : ITestPanelItemRepository
{
    private readonly ApplicationDbContext _context;

    public TestPanelItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TestPanelItem>> GetByPanelIdAsync(
        Guid testPanelId,
        CancellationToken cancellationToken = default)
    {
        return await _context.TestPanelItems
            .Include(x => x.Test)
            .Where(x => x.TestPanelId == testPanelId)
            .OrderBy(x => x.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<TestPanelItem?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.TestPanelItems
            .Include(x => x.Test)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task AddAsync(
        TestPanelItem entity,
        CancellationToken cancellationToken = default)
    {
        await _context.TestPanelItems.AddAsync(
            entity,
            cancellationToken);
    }

    public void Update(TestPanelItem entity)
    {
        _context.TestPanelItems.Update(entity);
    }

    public void Delete(TestPanelItem entity)
    {
        _context.TestPanelItems.Remove(entity);
    }
}
