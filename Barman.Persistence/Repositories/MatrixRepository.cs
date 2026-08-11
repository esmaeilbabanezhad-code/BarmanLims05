using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class MatrixRepository : IMatrixRepository
{
    private readonly ApplicationDbContext _context;

    public MatrixRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Matrix>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Matrices
            .Include(x => x.SampleCategory)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Matrix?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Matrices
            .Include(x => x.SampleCategory)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<List<Matrix>> GetBySampleCategoryIdAsync(
        Guid sampleCategoryId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Matrices
            .Include(x => x.SampleCategory)
            .Where(x => x.SampleCategoryId == sampleCategoryId)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Matrix>> GetDeletedAsync(
    CancellationToken cancellationToken = default)
    {
        return await _context.Matrices
            .Include(x => x.SampleCategory)
            .Where(x => x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Matrix?> GetDeletedByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Matrices
            .Include(x => x.SampleCategory)
            .FirstOrDefaultAsync(
                x => x.Id == id && x.IsDeleted,
                cancellationToken);
    }

    public async Task AddAsync(
        Matrix matrix,
        CancellationToken cancellationToken = default)
    {
        await _context.Matrices.AddAsync(
            matrix,
            cancellationToken);
    }

    public void Update(Matrix matrix)
    {
        _context.Matrices.Update(matrix);
    }

    public void Delete(Matrix matrix)
    {
        _context.Matrices.Remove(matrix);
    }
}