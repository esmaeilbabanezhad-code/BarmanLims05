using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class TestResultDefinitionRepository
    : ITestResultDefinitionRepository
{
    private readonly ApplicationDbContext _context;

    public TestResultDefinitionRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TestResultDefinition>> GetByTestIdAsync(
        Guid testId,
        CancellationToken cancellationToken = default)
    {
        return await _context.TestResultDefinitions
            .Where(x =>
                x.TestId == testId &&
                !x.IsDeleted)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<TestResultDefinition?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.TestResultDefinitions
            .FirstOrDefaultAsync(
                x => x.Id == id &&
                     !x.IsDeleted,
                cancellationToken);
    }

    public async Task AddAsync(
        TestResultDefinition entity,
        CancellationToken cancellationToken = default)
    {
        await _context.TestResultDefinitions.AddAsync(
            entity,
            cancellationToken);
    }

    public void Update(TestResultDefinition entity)
    {
        _context.TestResultDefinitions.Update(entity);
    }

    public void Delete(TestResultDefinition entity)
    {
        _context.TestResultDefinitions.Remove(entity);
    }
}