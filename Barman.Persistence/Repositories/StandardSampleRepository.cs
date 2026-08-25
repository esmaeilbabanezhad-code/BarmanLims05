using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class StandardSampleRepository : IStandardSampleRepository
{
    private readonly ApplicationDbContext _context;

    public StandardSampleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<StandardSample>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.StandardSamples
            .Include(x => x.SampleCategory)
            .Include(x => x.Matrix)
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<StandardSample?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.StandardSamples
            .Include(x => x.SampleCategory)
            .Include(x => x.Matrix)
            .FirstOrDefaultAsync(
                x => x.Id == id && !x.IsDeleted,
                cancellationToken);
    }

    public async Task<List<StandardSample>> GetByMatrixIdAsync(
        Guid matrixId,
        CancellationToken cancellationToken = default)
    {
        return await _context.StandardSamples
            .Include(x => x.SampleCategory)
            .Include(x => x.Matrix)
            .Where(x =>
                x.MatrixId == matrixId &&
                !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<StandardSample>> GetDeletedAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.StandardSamples
            .Include(x => x.SampleCategory)
            .Include(x => x.Matrix)
            .Where(x => x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<StandardSample?> GetDeletedByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.StandardSamples
            .Include(x => x.SampleCategory)
            .Include(x => x.Matrix)
            .FirstOrDefaultAsync(
                x => x.Id == id && x.IsDeleted,
                cancellationToken);
    }

    public async Task AddAsync(
        StandardSample standardSample,
        CancellationToken cancellationToken = default)
    {
        await _context.StandardSamples.AddAsync(
            standardSample,
            cancellationToken);
    }

    public void Update(StandardSample standardSample)
    {
        _context.StandardSamples.Update(standardSample);
    }

    public void Delete(StandardSample standardSample)
    {
        _context.StandardSamples.Remove(standardSample);
    }
}