using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class CustomerTestPanelRepository : ICustomerTestPanelRepository
{
    private readonly ApplicationDbContext _context;

    public CustomerTestPanelRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CustomerTestPanel>> GetByCustomerAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        return await _context.CustomerTestPanels
            .Include(x => x.SampleCategory)
            .Include(x => x.Matrix)
            .Include(x => x.TestPanel)
                .ThenInclude(x => x.Items)
                    .ThenInclude(x => x.Test)
            .Where(x =>
                x.CustomerId == customerId &&
                !x.IsDeleted &&
                x.IsActive)
            .OrderBy(x => x.Priority)
            .ToListAsync(cancellationToken);
    }

    public async Task<CustomerTestPanel?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.CustomerTestPanels
            .Include(x => x.SampleCategory)
            .Include(x => x.Matrix)
            .Include(x => x.TestPanel)
                .ThenInclude(x => x.Items)
                    .ThenInclude(x => x.Test)
            .FirstOrDefaultAsync(
                x => x.Id == id &&
                     !x.IsDeleted &&
                     x.IsActive,
                cancellationToken);
    }

    public async Task<CustomerTestPanel?> ResolveAsync(
    Guid customerId,
    Guid? sampleCategoryId,
    Guid? matrixId,
    CancellationToken cancellationToken = default)
    {
        if (customerId == Guid.Empty)
            return null;

        var rules = await _context.CustomerTestPanels
            .Include(x => x.SampleCategory)
            .Include(x => x.Matrix)
            .Include(x => x.TestPanel)
                .ThenInclude(x => x.Items)
                    .ThenInclude(x => x.Test)
            .Where(x =>
                x.CustomerId == customerId &&
                !x.IsDeleted &&
                x.IsActive &&
                (
                    x.SampleCategoryId == sampleCategoryId ||
                    x.SampleCategoryId == null
                ) &&
                (
                    x.MatrixId == matrixId ||
                    x.MatrixId == null
                ))
            .ToListAsync(cancellationToken);

        return rules
            .OrderByDescending(x =>
                x.SampleCategoryId.HasValue &&
                sampleCategoryId.HasValue &&
                x.SampleCategoryId == sampleCategoryId)
            .ThenByDescending(x =>
                x.MatrixId.HasValue &&
                matrixId.HasValue &&
                x.MatrixId == matrixId)
            .ThenBy(x => x.Priority)
            .FirstOrDefault();
    }

    public async Task AddAsync(
        CustomerTestPanel entity,
        CancellationToken cancellationToken = default)
    {
        await _context.CustomerTestPanels.AddAsync(
            entity,
            cancellationToken);
    }

    public void Update(CustomerTestPanel entity)
    {
        _context.CustomerTestPanels.Update(entity);
    }

    public void Delete(CustomerTestPanel entity)
    {
        _context.CustomerTestPanels.Remove(entity);
    }
}
