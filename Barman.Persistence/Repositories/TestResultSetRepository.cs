using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Barman.Application.Interfaces;

namespace Barman.Persistence.Repositories;

public class TestResultSetRepository : ITestResultSetRepository
{
    private readonly ApplicationDbContext _context;

    private readonly IScopedTemplateResolver _scopedTemplateResolver;

    public TestResultSetRepository(
    ApplicationDbContext context,
    IScopedTemplateResolver scopedTemplateResolver)
    {
        _context = context;
        _scopedTemplateResolver = scopedTemplateResolver;
    }

    public async Task<List<TestResultSet>> GetByTestIdAsync(
        Guid testId,
        CancellationToken cancellationToken = default)
    {
        return await _context.TestResultSets
            .Include(x => x.Test)
            .Include(x => x.Customer)
            .Include(x => x.SampleCategory)
            .Include(x => x.Matrix)
            .Include(x => x.StandardSample)
            .Include(x => x.Items)
                .ThenInclude(x => x.TestResultDefinition)
            .Where(x =>
                x.TestId == testId &&
                !x.IsDeleted)
            .OrderBy(x => x.Priority)
            .ThenBy(x => x.Code)
            .ToListAsync(cancellationToken);
    }
    public async Task<TestResultSet?> ResolveAsync(
    Guid testId,
    Guid? customerId,
    Guid? sampleCategoryId,
    Guid? matrixId,
    Guid? standardSampleId,
    CancellationToken cancellationToken = default)
    {
        var query = _context.TestResultSets
            .Include(x => x.Test)
            .Include(x => x.Customer)
            .Include(x => x.SampleCategory)
            .Include(x => x.Matrix)
            .Include(x => x.StandardSample)
            .Include(x => x.Items)
                .ThenInclude(x => x.TestResultDefinition)
            .Where(x =>
                x.TestId == testId &&
                !x.IsDeleted &&
                x.IsActive);

        // null = General / applies to all
        query = query.Where(x =>
            (x.CustomerId == null ||
             x.CustomerId == customerId) &&

            (x.SampleCategoryId == null ||
             x.SampleCategoryId == sampleCategoryId) &&

            (x.MatrixId == null ||
             x.MatrixId == matrixId) &&

            (x.StandardSampleId == null ||
             x.StandardSampleId == standardSampleId));

        var sets = await query
            .ToListAsync(cancellationToken);

        return sets
            .OrderByDescending(x =>
                _scopedTemplateResolver.Specificity(
                    x.StandardSampleId,
                    standardSampleId))

            .ThenByDescending(x =>
                _scopedTemplateResolver.Specificity(
                    x.CustomerId,
                    customerId))

            .ThenByDescending(x =>
                _scopedTemplateResolver.Specificity(
                    x.SampleCategoryId,
                    sampleCategoryId))

            .ThenByDescending(x =>
                _scopedTemplateResolver.Specificity(
                    x.MatrixId,
                    matrixId))

            .ThenBy(x => x.Priority)

            .FirstOrDefault();
    }
    public async Task<TestResultSet?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.TestResultSets
            .Include(x => x.Test)
            .Include(x => x.Customer)
            .Include(x => x.SampleCategory)
            .Include(x => x.Matrix)
            .Include(x => x.StandardSample)
            .Include(x => x.Items)
                .ThenInclude(x => x.TestResultDefinition)
            .FirstOrDefaultAsync(
                x => x.Id == id &&
                     !x.IsDeleted,
                cancellationToken);
    }

    public async Task AddAsync(
        TestResultSet entity,
        CancellationToken cancellationToken = default)
    {
        await _context.TestResultSets.AddAsync(
            entity,
            cancellationToken);
    }

    public void Update(TestResultSet entity)
    {
        _context.TestResultSets.Update(entity);
    }

    public void Delete(TestResultSet entity)
    {
        _context.TestResultSets.Remove(entity);
    }
}