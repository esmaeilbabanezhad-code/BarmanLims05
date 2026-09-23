using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Barman.Application.Interfaces;
using Barman.Application.Services.Resolvers;

namespace Barman.Persistence.Repositories;

public class DefaultTestSetRepository : IDefaultTestSetRepository
{
    private readonly ApplicationDbContext _context;

    private readonly IScopedTemplateResolver _scopedTemplateResolver;

    public DefaultTestSetRepository(
    ApplicationDbContext context,
    IScopedTemplateResolver scopedTemplateResolver)
    {
        _context = context;
        _scopedTemplateResolver = scopedTemplateResolver;
    }

    public async Task<List<DefaultTestSet>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.DefaultTestSets
            .Include(x => x.Customer)
            .Include(x => x.SampleCategory)
            .Include(x => x.Matrix)
            .Include(x => x.Items)
                .ThenInclude(x => x.Test)
            .Include(x => x.Items)
                .ThenInclude(x => x.TestPanel)
            .Where(x =>
                !x.IsDeleted &&
                x.IsActive)
            .OrderBy(x => x.Priority)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<DefaultTestSet>> GetByCustomerAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        if (customerId == Guid.Empty)
            return new List<DefaultTestSet>();

        return await _context.DefaultTestSets
            .Include(x => x.Customer)
            .Include(x => x.SampleCategory)
            .Include(x => x.Matrix)
            .Include(x => x.Items)
                .ThenInclude(x => x.Test)
            .Include(x => x.Items)
                .ThenInclude(x => x.TestPanel)
            .Where(x =>
                x.CustomerId == customerId &&
                !x.IsDeleted &&
                x.IsActive)
            .OrderBy(x => x.Priority)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<DefaultTestSet?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return null;

        return await _context.DefaultTestSets
            .Include(x => x.Customer)
            .Include(x => x.SampleCategory)
            .Include(x => x.Matrix)
            .Include(x => x.Items)
                .ThenInclude(x => x.Test)
            .Include(x => x.Items)
                .ThenInclude(x => x.TestPanel)
            .FirstOrDefaultAsync(
                x =>
                    x.Id == id &&
                    !x.IsDeleted &&
                    x.IsActive,
                cancellationToken);
    }

    public async Task<DefaultTestSet?> ResolveAsync(
    Guid? customerId,
    Guid? sampleCategoryId,
    Guid? matrixId,
    Guid? standardSampleId,
    CancellationToken cancellationToken = default)
    {
        var query = _context.DefaultTestSets
            .Include(x => x.Customer)
            .Include(x => x.SampleCategory)
            .Include(x => x.Matrix)
            .Include(x => x.StandardSample)
            .Include(x => x.Items)
                .ThenInclude(x => x.Test)
            .Include(x => x.Items)
                .ThenInclude(x => x.TestPanel)
            .Where(x =>
                !x.IsDeleted &&
                x.IsActive);

        // -------------------------------------------------
        // Scope matching
        // null = General / applies to all
        // -------------------------------------------------

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

        // -------------------------------------------------
        // Central specificity order:
        //
        // 1. Standard Sample
        // 2. Customer
        // 3. Sample Category
        // 4. Matrix
        // 5. Priority
        // -------------------------------------------------

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

    public async Task AddAsync(
        DefaultTestSet entity,
        CancellationToken cancellationToken = default)
    {
        await _context.DefaultTestSets.AddAsync(
            entity,
            cancellationToken);
    }

    public void Update(DefaultTestSet entity)
    {
        _context.DefaultTestSets.Update(entity);
    }

    public void Delete(DefaultTestSet entity)
    {
        _context.DefaultTestSets.Remove(entity);
    }
}