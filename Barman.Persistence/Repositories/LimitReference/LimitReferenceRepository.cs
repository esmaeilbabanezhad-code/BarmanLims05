using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class LimitReferenceRepository : ILimitReferenceRepository
{
    private readonly ApplicationDbContext _context;

    public LimitReferenceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LimitReference?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.LimitReferences
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<List<LimitReference>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.LimitReferences
            .OrderBy(x => x.Priority)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        LimitReference limitReference)
    {
        await _context.LimitReferences
            .AddAsync(limitReference);
    }

    public void Update(LimitReference limitReference)
    {
        _context.LimitReferences.Update(limitReference);
    }

    public void Delete(LimitReference limitReference)
    {
        limitReference.IsDeleted = true;
        limitReference.IsActive = false;

        _context.LimitReferences.Update(limitReference);
    }

    public async Task<LimitReference?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return await _context.LimitReferences
            .FirstOrDefaultAsync(
                x => x.Code == code,
                cancellationToken);
    }

    public async Task<List<LimitReference>> GetDeletedAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.LimitReferences
            .AsNoTracking()
            .Where(x => x.IsDeleted)
            .OrderBy(x => x.Priority)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<LimitReference?> GetDeletedByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.LimitReferences
            .FirstOrDefaultAsync(
                x => x.Id == id && x.IsDeleted,
                cancellationToken);
    }
}
