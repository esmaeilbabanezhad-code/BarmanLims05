using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class InstrumentRepository : IInstrumentRepository
{
    private readonly ApplicationDbContext _context;

    public InstrumentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Instrument instrument)
    {
        await _context.Instruments.AddAsync(instrument);
    }

    public async Task<Instrument?> GetByIdAsync(Guid id)
    {
        return await _context.Instruments
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Instrument?> GetByCodeAsync(string code)
    {
        return await _context.Instruments
            .FirstOrDefaultAsync(x => x.Code == code);
    }

    public async Task<List<Instrument>> GetAllAsync()
    {
        return await _context.Instruments
            .AsNoTracking()
            .ToListAsync();
    }

    public void Update(Instrument instrument)
    {
        _context.Instruments.Update(instrument);
    }

    public void Delete(Instrument instrument)
    {
        _context.Instruments.Remove(instrument);
    }
}
