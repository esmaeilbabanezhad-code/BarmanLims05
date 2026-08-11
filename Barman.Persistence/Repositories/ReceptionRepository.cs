using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class ReceptionRepository : IReceptionRepository
{
    private readonly ApplicationDbContext _context;

    public ReceptionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Reception reception)
    {
        await _context.Receptions.AddAsync(reception);
    }

    public async Task<Reception?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Receptions
            .Include(x => x.Customer)
            .Include(x => x.Samples)
                .ThenInclude(x => x.SampleCategory)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<List<Reception>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Receptions
            .Include(x => x.Customer)
            .Include(x => x.Samples)
                .ThenInclude(x => x.SampleCategory)
            .OrderByDescending(x => x.ReceptionDate)
            .ToListAsync(cancellationToken);
    }

    public IQueryable<Reception> Query()
    {
        return _context.Receptions
            .AsQueryable();
    }
}