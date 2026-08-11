using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class SampleRepository : ISampleRepository
{
    private readonly ApplicationDbContext _context;

    public SampleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Sample sample)
    {
        await _context.Samples.AddAsync(sample);
    }

    public async Task<Sample?> GetByIdAsync(Guid id)
    {
        return await _context.Samples
            .Include(x => x.Reception)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Sample>> GetByReceptionIdAsync(Guid receptionId)
    {
        return await _context.Samples
            .Where(x => x.ReceptionId == receptionId)
            .ToListAsync();
    }

    public void Update(Sample sample)
    {
        _context.Samples.Update(sample);
    }

    public void Delete(Sample sample)
    {
        _context.Samples.Remove(sample);
    }
}