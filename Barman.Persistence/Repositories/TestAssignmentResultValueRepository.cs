using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class TestAssignmentResultValueRepository
    : ITestAssignmentResultValueRepository
{
    private readonly ApplicationDbContext _context;

    public TestAssignmentResultValueRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TestAssignmentResultValue>>
        GetByAssignmentIdAsync(Guid testAssignmentId)
    {
        return await _context.TestAssignmentResultValues
            .Where(x =>
                x.TestAssignmentId == testAssignmentId &&
                !x.IsDeleted)
            .Include(x => x.TestResultSetItem)
                .ThenInclude(x => x.TestResultDefinition)
            .OrderBy(x =>
                x.TestResultSetItem.DisplayOrder)
            .ToListAsync();
    }
    public async Task<List<TestAssignmentResultValue>> GetByAssignmentIdsAsync(
        IReadOnlyCollection<Guid> assignmentIds)
    {
        if (assignmentIds is null ||
            assignmentIds.Count == 0)
        {
            return new List<TestAssignmentResultValue>();
        }

        return await _context.TestAssignmentResultValues
            .Where(x =>
                assignmentIds.Contains(x.TestAssignmentId) &&
                !x.IsDeleted)
            .Include(x => x.TestResultSetItem)
                .ThenInclude(x => x.TestResultDefinition)
            .OrderBy(x => x.TestAssignmentId)
            .ThenBy(x => x.TestResultSetItem.DisplayOrder)
            .ToListAsync();
    }
    public async Task<TestAssignmentResultValue?>
        GetByAssignmentAndItemAsync(
            Guid testAssignmentId,
            Guid testResultSetItemId)
    {
        return await _context.TestAssignmentResultValues
            .Include(x => x.TestResultSetItem)
                .ThenInclude(x => x.TestResultDefinition)
            .FirstOrDefaultAsync(x =>
                x.TestAssignmentId == testAssignmentId &&
                x.TestResultSetItemId == testResultSetItemId &&
                !x.IsDeleted);
    }
    public async Task<TestAssignmentResultValue?>
    GetByIdAsync(Guid id)
    {
        return await _context.TestAssignmentResultValues
            .Include(x => x.TestResultSetItem)
                .ThenInclude(x => x.TestResultDefinition)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                !x.IsDeleted);
    }
    public async Task AddAsync(
        TestAssignmentResultValue resultValue)
    {
        await _context.TestAssignmentResultValues
            .AddAsync(resultValue);
    }

    public void Update(
        TestAssignmentResultValue resultValue)
    {
        _context.TestAssignmentResultValues
            .Update(resultValue);
    }

    public void Delete(
        TestAssignmentResultValue resultValue)
    {
        resultValue.IsDeleted = true;

        _context.TestAssignmentResultValues
            .Update(resultValue);
    }
}
