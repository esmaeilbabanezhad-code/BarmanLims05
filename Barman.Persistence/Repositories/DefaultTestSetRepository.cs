using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class DefaultTestSetRepository : IDefaultTestSetRepository
{
    private readonly ApplicationDbContext _context;

    public DefaultTestSetRepository(ApplicationDbContext context)
    {
        _context = context;
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
        CancellationToken cancellationToken = default)
    {
        var query = _context.DefaultTestSets
            .Include(x => x.Customer)
            .Include(x => x.SampleCategory)
            .Include(x => x.Matrix)
            .Include(x => x.Items)
                .ThenInclude(x => x.Test)
            .Include(x => x.Items)
                .ThenInclude(x => x.TestPanel)
            .Where(x =>
                !x.IsDeleted &&
                x.IsActive);

        if (customerId.HasValue &&
            customerId.Value != Guid.Empty)
        {
            query = query.Where(x =>
                x.CustomerId == customerId ||
                x.CustomerId == null);
        }
        else
        {
            query = query.Where(x =>
                x.CustomerId == null);
        }

        query = query.Where(x =>
            x.SampleCategoryId == sampleCategoryId ||
            x.SampleCategoryId == null);

        query = query.Where(x =>
            x.MatrixId == matrixId ||
            x.MatrixId == null);

        var sets = await query.ToListAsync(cancellationToken);

        return sets
            .OrderByDescending(x =>
                x.CustomerId.HasValue &&
                customerId.HasValue &&
                x.CustomerId == customerId)
            .ThenByDescending(x =>
                x.SampleCategoryId.HasValue &&
                sampleCategoryId.HasValue &&
                x.SampleCategoryId == sampleCategoryId)
            .ThenByDescending(x =>
                x.MatrixId.HasValue &&
                matrixId.HasValue &&
                x.MatrixId == matrixId)
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