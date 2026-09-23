using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class TestLimitRuleRepository : ITestLimitRuleRepository
{
    private readonly ApplicationDbContext _context;

    public TestLimitRuleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TestLimitRule?> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            return null;

        return await _context.TestLimitRules
            .Include(x => x.Test)
            .Include(x => x.Matrix)
            .Include(x => x.SampleCategory)
            .Include(x => x.Customer)
            .Include(x => x.LimitReference)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<TestLimitRule>> GetApplicableAsync(
    Guid testId,
    Guid? customerId,
    Guid? matrixId,
    Guid? sampleCategoryId)
    {
        if (testId == Guid.Empty)
            return new List<TestLimitRule>();

        return await _context.TestLimitRules
            .Where(x =>
                x.TestId == testId &&
                x.IsActive &&
                !x.IsDeleted &&
                x.ValidFrom <= DateTime.UtcNow &&
                (!x.ValidTo.HasValue || x.ValidTo.Value >= DateTime.UtcNow) &&
                (!x.CustomerId.HasValue || x.CustomerId == customerId) &&
                (!x.MatrixId.HasValue || x.MatrixId == matrixId) &&
                (!x.SampleCategoryId.HasValue || x.SampleCategoryId == sampleCategoryId))
            .Include(x => x.Test)
            .Include(x => x.Matrix)
            .Include(x => x.SampleCategory)
            .Include(x => x.Customer)
            .Include(x => x.LimitReference)
            .OrderByDescending(x => x.Priority)
            .ThenByDescending(x => x.ValidFrom)
            .ToListAsync();
    }

    public async Task AddAsync(TestLimitRule rule)
    {
        await _context.TestLimitRules.AddAsync(rule);
    }

    public void Update(TestLimitRule rule)
    {
        _context.TestLimitRules.Update(rule);
    }

    public void Delete(TestLimitRule rule)
    {
        _context.TestLimitRules.Remove(rule);
    }
    public async Task<List<TestLimitRule>> GetByTestIdAsync(Guid testId)
    {
        if (testId == Guid.Empty)
            return new List<TestLimitRule>();

        return await _context.TestLimitRules
            .Where(x => x.TestId == testId && !x.IsDeleted)
            .Include(x => x.LimitReference)
            .Include(x => x.Test)
            .OrderBy(x => x.Priority)
            .ToListAsync();
    }
}

