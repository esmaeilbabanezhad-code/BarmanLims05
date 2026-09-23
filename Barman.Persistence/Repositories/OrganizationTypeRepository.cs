using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class OrganizationTypeRepository : IOrganizationTypeRepository
{
    private readonly ApplicationDbContext _context;

    public OrganizationTypeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OrganizationType?> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            return null;

        return await _context.OrganizationTypes
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
    }

    public async Task<List<OrganizationType>> GetAllAsync()
    {
        return await _context.OrganizationTypes
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<List<OrganizationType>> GetDeletedAsync()
    {
        return await _context.OrganizationTypes
            .Where(x => x.IsDeleted)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<OrganizationType?> GetDeletedByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            return null;

        return await _context.OrganizationTypes
            .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted);
    }

    public async Task AddAsync(OrganizationType organizationType)
    {
        await _context.OrganizationTypes.AddAsync(organizationType);
    }

    public void Update(OrganizationType organizationType)
    {
        _context.OrganizationTypes.Update(organizationType);
    }

    public void Delete(OrganizationType organizationType)
    {
        _context.OrganizationTypes.Remove(organizationType);
    }
}