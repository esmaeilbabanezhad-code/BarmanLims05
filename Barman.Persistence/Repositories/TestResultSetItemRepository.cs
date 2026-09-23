using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class TestResultSetItemRepository
    : ITestResultSetItemRepository
{
    private readonly ApplicationDbContext _context;

    public TestResultSetItemRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TestResultSetItem>>
        GetByResultSetIdAsync(
            Guid testResultSetId,
            CancellationToken cancellationToken = default)
    {
        return await _context.TestResultSetItems
            .Include(x => x.TestResultDefinition)
            .Where(x =>
                x.TestResultSetId == testResultSetId &&
                !x.IsDeleted)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<TestResultSetItem?>
        GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
    {
        return await _context.TestResultSetItems
                .Include(x => x.TestResultDefinition)
                .Include(x => x.TestResultSet)
                .FirstOrDefaultAsync(
        x => x.Id == id &&
             !x.IsDeleted,
        cancellationToken);
    }

    public async Task AddAsync(
        TestResultSetItem entity,
        CancellationToken cancellationToken = default)
    {
        await _context.TestResultSetItems.AddAsync(
            entity,
            cancellationToken);
    }

    public void Update(TestResultSetItem entity)
    {
        _context.TestResultSetItems.Update(entity);
    }

    public void Delete(TestResultSetItem entity)
    {
        _context.TestResultSetItems.Remove(entity);
    }
}