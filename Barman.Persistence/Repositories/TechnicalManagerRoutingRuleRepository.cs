using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class TechnicalManagerRoutingRuleRepository
    : ITechnicalManagerRoutingRuleRepository
{
    private readonly ApplicationDbContext _context;

    public TechnicalManagerRoutingRuleRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TechnicalManagerRoutingRule>>
        GetAllAsync()
    {
        return await _context.TechnicalManagerRoutingRules
            .Include(x => x.Customer)
            .Include(x => x.SampleCategory)
            .Include(x => x.Matrix)
            .Include(x => x.TestPanel)
            .Include(x => x.Test)
            .Include(x => x.Department)
            .Include(x => x.TechnicalManagerScope)
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Priority)
            .ThenBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<TechnicalManagerRoutingRule?>
        GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            return null;

        return await _context.TechnicalManagerRoutingRules
            .Include(x => x.Customer)
            .Include(x => x.SampleCategory)
            .Include(x => x.Matrix)
            .Include(x => x.TestPanel)
            .Include(x => x.Test)
            .Include(x => x.Department)
            .Include(x => x.TechnicalManagerScope)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                !x.IsDeleted);
    }

    public async Task<List<TechnicalManagerRoutingRule>>
        GetApplicableAsync(
            Guid? customerId,
            Guid? sampleCategoryId,
            Guid? matrixId,
            Guid? testPanelId,
            Guid? testId,
            Guid? departmentId)
    {
        var query =
            _context.TechnicalManagerRoutingRules
                .Include(x => x.TechnicalManagerScope)
                .Where(x =>
                    !x.IsDeleted &&
                    x.IsActive);

        query = query.Where(x =>
            (!x.CustomerId.HasValue ||
             x.CustomerId == customerId) &&

            (!x.SampleCategoryId.HasValue ||
             x.SampleCategoryId == sampleCategoryId) &&

            (!x.MatrixId.HasValue ||
             x.MatrixId == matrixId) &&

            (!x.TestPanelId.HasValue ||
             x.TestPanelId == testPanelId) &&

            (!x.TestId.HasValue ||
             x.TestId == testId) &&

            (!x.DepartmentId.HasValue ||
             x.DepartmentId == departmentId));

        return await query
            .OrderBy(x => x.Priority)
            .ThenBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<bool> ExistsByCodeAsync(
        string code,
        Guid? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            return false;

        var normalizedCode = code.Trim();

        return await _context.TechnicalManagerRoutingRules
            .AnyAsync(x =>
                x.Code == normalizedCode &&
                !x.IsDeleted &&
                (!excludeId.HasValue ||
                 x.Id != excludeId.Value));
    }

    public async Task AddAsync(
        TechnicalManagerRoutingRule rule)
    {
        await _context.TechnicalManagerRoutingRules
            .AddAsync(rule);
    }

    public void Update(
        TechnicalManagerRoutingRule rule)
    {
        _context.TechnicalManagerRoutingRules
            .Update(rule);
    }

    public void Delete(
        TechnicalManagerRoutingRule rule)
    {
        _context.TechnicalManagerRoutingRules
            .Remove(rule);
    }
}