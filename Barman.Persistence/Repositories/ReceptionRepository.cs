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
            .Include(x => x.Samples)
                .ThenInclude(x => x.Matrix)
            .Include(x => x.Samples)
                .ThenInclude(x => x.StandardSample)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }
    public async Task<Reception?> GetForCorrectionAsync(
    Guid id,
    CancellationToken cancellationToken = default)
    {
        return await _context.Receptions
            .Include(x => x.Customer)
            .Include(x => x.Samples)
                .ThenInclude(x => x.SampleCategory)
            .Include(x => x.Samples)
                .ThenInclude(x => x.Matrix)
            .Include(x => x.Samples)
                .ThenInclude(x => x.StandardSample)
            .Include(x => x.Samples)
                .ThenInclude(x => x.TestAssignments)
                    .ThenInclude(x => x.Test)
            .Include(x => x.Samples)
                .ThenInclude(x => x.TestAssignments)
                    .ThenInclude(x => x.TestPanel)
            .Include(x => x.Samples)
                .ThenInclude(x => x.TestAssignments)
                    .ThenInclude(x => x.Department)
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

    public async Task<List<Reception>> GetHistoryByCustomerIdAsync(
    Guid customerId,
    CancellationToken cancellationToken = default)
    {
        return await _context.Receptions
            .Include(x => x.Customer)
            .Include(x => x.Samples)
                .ThenInclude(x => x.TestAssignments)
                    .ThenInclude(x => x.Test)
            .Include(x => x.Samples)
                .ThenInclude(x => x.TestAssignments)
                    .ThenInclude(x => x.TestPanel)
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.ReceptionDate)
            .ToListAsync(cancellationToken);
    }

    public IQueryable<Reception> Query()
    {
        return _context.Receptions
            .AsQueryable();
    }
}