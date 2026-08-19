using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class TechnicalManagerSectionHeadRepository
    : ITechnicalManagerSectionHeadRepository
{
    private readonly ApplicationDbContext _context;

    public TechnicalManagerSectionHeadRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TechnicalManagerSectionHead>>
        GetByTechnicalManagerIdAsync(
            Guid technicalManagerId)
    {
        return await _context
            .Set<TechnicalManagerSectionHead>()
            .Include(x => x.TechnicalManager)
            .Include(x => x.SectionHead)
            .Where(x => x.TechnicalManagerId == technicalManagerId)
            .ToListAsync();
    }

    public async Task<List<TechnicalManagerSectionHead>>
        GetBySectionHeadIdAsync(
            Guid sectionHeadId)
    {
        return await _context
            .Set<TechnicalManagerSectionHead>()
            .Include(x => x.TechnicalManager)
            .Include(x => x.SectionHead)
            .Where(x => x.SectionHeadId == sectionHeadId)
            .ToListAsync();
    }

    public async Task<TechnicalManagerSectionHead?> GetByIdAsync(
        Guid id)
    {
        return await _context
            .Set<TechnicalManagerSectionHead>()
            .Include(x => x.TechnicalManager)
            .Include(x => x.SectionHead)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(
        TechnicalManagerSectionHead relation)
    {
        await _context
            .Set<TechnicalManagerSectionHead>()
            .AddAsync(relation);
    }

    public void Update(
        TechnicalManagerSectionHead relation)
    {
        _context
            .Set<TechnicalManagerSectionHead>()
            .Update(relation);
    }

    public void Delete(
        TechnicalManagerSectionHead relation)
    {
        _context
            .Set<TechnicalManagerSectionHead>()
            .Remove(relation);
    }
}
