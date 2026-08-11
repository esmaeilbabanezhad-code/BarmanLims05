using Microsoft.EntityFrameworkCore;
using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;

namespace Barman.Persistence.Repositories;

public class NumberSequenceRepository : INumberSequenceRepository
{
    private readonly ApplicationDbContext _context;

    public NumberSequenceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<NumberSequence?> GetByEntityNameAsync(string entityName)
    {
        return await _context.NumberSequences
            .FirstOrDefaultAsync(x => x.EntityName == entityName);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}