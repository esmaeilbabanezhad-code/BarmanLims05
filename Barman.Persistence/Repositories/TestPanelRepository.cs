using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class TestPanelRepository : ITestPanelRepository
{
    private readonly ApplicationDbContext _context;

    public TestPanelRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TestPanel>> GetAllAsync(
    CancellationToken cancellationToken = default)
    {
        return await _context.TestPanels
            .Include(x => x.Items)
            .ThenInclude(x => x.Test)
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<TestPanel?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.TestPanels
            .Include(x => x.Items)
            .ThenInclude(x => x.Test)
            .FirstOrDefaultAsync(
                x => x.Id == id &&
                     !x.IsDeleted,
                cancellationToken);
    }

    public async Task AddAsync(
        TestPanel entity,
        CancellationToken cancellationToken = default)
    {
        await _context.TestPanels.AddAsync(
            entity,
            cancellationToken);
    }

    public void Update(TestPanel entity)
    {
        _context.TestPanels.Update(entity);
    }

    public void Delete(TestPanel entity)
    {
        _context.TestPanels.Remove(entity);
    }
}