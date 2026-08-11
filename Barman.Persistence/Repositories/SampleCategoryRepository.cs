using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class SampleCategoryRepository : ISampleCategoryRepository
{
    private readonly ApplicationDbContext _context;

    public SampleCategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SampleCategory?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.SampleCategories
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<List<SampleCategory>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.SampleCategories
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        SampleCategory category)
    {
        await _context.SampleCategories
            .AddAsync(category);
    }

    public void Update(SampleCategory category)
    {
        _context.SampleCategories.Update(category);
    }

    public void Delete(SampleCategory category)
    {
        category.IsDeleted = true;
        category.IsActive = false;

        _context.SampleCategories.Update(category);
    }

    public async Task<SampleCategory?> GetByCodeAsync(
    string code,
    CancellationToken cancellationToken = default)
    {
        return await _context.SampleCategories
            .FirstOrDefaultAsync(
                x => x.Code == code,
                cancellationToken);
    }

    public async Task<List<SampleCategory>> GetDeletedAsync(
    CancellationToken cancellationToken = default)
    {
        return await _context.SampleCategories
            .AsNoTracking()
            .Where(x => x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<SampleCategory?> GetDeletedByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.SampleCategories
            .FirstOrDefaultAsync(
                x => x.Id == id && x.IsDeleted,
                cancellationToken);
    }
}