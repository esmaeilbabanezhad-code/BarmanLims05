using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class TechnicalManagerScopeRepository
    : ITechnicalManagerScopeRepository
{
    private readonly ApplicationDbContext _context;

    public TechnicalManagerScopeRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TechnicalManagerScope>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.TechnicalManagerScopes
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Priority)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<TechnicalManagerScope?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return null;

        return await _context.TechnicalManagerScopes
            .FirstOrDefaultAsync(
                x => x.Id == id && !x.IsDeleted,
                cancellationToken);
    }

    public async Task<TechnicalManagerScope?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        var normalizedCode = code.Trim();

        return await _context.TechnicalManagerScopes
            .FirstOrDefaultAsync(
                x => x.Code == normalizedCode && !x.IsDeleted,
                cancellationToken);
    }

    public async Task AddAsync(
        TechnicalManagerScope scope,
        CancellationToken cancellationToken = default)
    {
        await _context.TechnicalManagerScopes.AddAsync(
            scope,
            cancellationToken);
    }

    public void Update(TechnicalManagerScope scope)
    {
        _context.TechnicalManagerScopes.Update(scope);
    }

    public void Delete(TechnicalManagerScope scope)
    {
        _context.TechnicalManagerScopes.Remove(scope);
    }
}