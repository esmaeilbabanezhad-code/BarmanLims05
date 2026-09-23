using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class ReferenceLimitRepository : IReferenceLimitRepository
{
    private readonly ApplicationDbContext _context;

    public ReferenceLimitRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ReferenceLimit?> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            return null;

        return await _context.ReferenceLimits
            .Include(x => x.Test)
            .Include(x => x.Matrix)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<ReferenceLimit>> GetByTestIdAsync(Guid testId)
    {
        if (testId == Guid.Empty)
            return new List<ReferenceLimit>();

        return await _context.ReferenceLimits
            .Where(x => x.TestId == testId)
            .Include(x => x.Test)
            .Include(x => x.Matrix)
            .OrderBy(x => x.Priority)
            .ToListAsync();
    }

    public async Task AddAsync(ReferenceLimit referenceLimit)
    {
        await _context.ReferenceLimits.AddAsync(referenceLimit);
    }
    public void Update(ReferenceLimit referenceLimit)
    {
        _context.ReferenceLimits.Update(referenceLimit);
    }
    public void Delete(ReferenceLimit referenceLimit)
    {
        _context.ReferenceLimits.Remove(referenceLimit);
    }
}