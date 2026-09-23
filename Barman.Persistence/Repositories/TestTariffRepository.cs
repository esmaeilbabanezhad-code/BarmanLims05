using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class TestTariffRepository : ITestTariffRepository
{
    private readonly ApplicationDbContext _context;

    public TestTariffRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TestTariff?> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            return null;

        return await _context.TestTariffs
            .Include(x => x.Test)
            .Include(x => x.TestPanel)
            .Include(x => x.OrganizationType)
            .Include(x => x.Customer)
            .Include(x => x.SampleCategory)
            .Include(x => x.Matrix)
            .Include(x => x.StandardSample)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<TestTariff>> GetAllAsync()
    {
        return await _context.TestTariffs
            .Where(x => !x.IsDeleted)
            .Include(x => x.Test)
            .Include(x => x.TestPanel)
            .Include(x => x.OrganizationType)
            .Include(x => x.Customer)
            .Include(x => x.SampleCategory)
            .Include(x => x.Matrix)
            .Include(x => x.StandardSample)
            .OrderBy(x => x.OrganizationType.DisplayOrder)
            .ThenBy(x => x.Priority)
            .ThenBy(x => x.ValidFrom)
            .ToListAsync();
    }

    public async Task<List<TestTariff>> GetByTestIdAsync(Guid testId)
    {
        if (testId == Guid.Empty)
            return new List<TestTariff>();

        return await _context.TestTariffs
            .Where(x => !x.IsDeleted && x.TestId == testId)
            .Include(x => x.OrganizationType)
            .Include(x => x.Customer)
            .Include(x => x.StandardSample)
            .Include(x => x.Matrix)
            .OrderBy(x => x.Priority)
            .ThenByDescending(x => x.ValidFrom)
            .ToListAsync();
    }

    public async Task<List<TestTariff>> GetByTestPanelIdAsync(Guid testPanelId)
    {
        if (testPanelId == Guid.Empty)
            return new List<TestTariff>();

        return await _context.TestTariffs
            .Where(x => !x.IsDeleted && x.TestPanelId == testPanelId)
            .Include(x => x.OrganizationType)
            .Include(x => x.Customer)
            .OrderBy(x => x.Priority)
            .ThenByDescending(x => x.ValidFrom)
            .ToListAsync();
    }

    public async Task<List<TestTariff>> GetByOrganizationTypeIdAsync(
        Guid organizationTypeId)
    {
        if (organizationTypeId == Guid.Empty)
            return new List<TestTariff>();

        return await _context.TestTariffs
            .Where(x =>
                !x.IsDeleted &&
                x.OrganizationTypeId == organizationTypeId)
            .Include(x => x.Test)
            .Include(x => x.TestPanel)
            .Include(x => x.Customer)
            .Include(x => x.StandardSample)
            .Include(x => x.Matrix)
            .OrderBy(x => x.Priority)
            .ThenByDescending(x => x.ValidFrom)
            .ToListAsync();
    }

    public async Task<TestTariff?> GetApplicableAsync(
    Guid organizationTypeId,
    Guid? customerId,
    Guid? standardSampleId,
    Guid? matrixId,
    Guid? testId,
    Guid? testPanelId,
    DateTime effectiveDate)
    {
        if (organizationTypeId == Guid.Empty)
            return null;

        if (!testId.HasValue && !testPanelId.HasValue)
            return null;

        var query = _context.TestTariffs
            .Where(x =>
                !x.IsDeleted &&
                x.IsActive &&
                x.OrganizationTypeId == organizationTypeId &&
                x.ValidFrom <= effectiveDate &&
                (!x.ValidTo.HasValue ||
                 x.ValidTo.Value >= effectiveDate));

        if (testId.HasValue)
        {
            query = query.Where(x => x.TestId == testId);
        }
        else
        {
            query = query.Where(x => x.TestPanelId == testPanelId);
        }

        query = query.Where(x =>
            x.CustomerId == customerId ||
            x.CustomerId == null);

        query = query.Where(x =>
            x.StandardSampleId == standardSampleId ||
            x.StandardSampleId == null);

        query = query.Where(x =>
            x.MatrixId == matrixId ||
            x.MatrixId == null);

        return await query
     .OrderByDescending(x =>
         (x.StandardSampleId.HasValue && x.StandardSampleId == standardSampleId ? 4 : 0) +
         (x.MatrixId.HasValue && x.MatrixId == matrixId ? 2 : 0) +
         (x.CustomerId.HasValue && x.CustomerId == customerId ? 1 : 0))
     .ThenBy(x => x.Priority)
     .ThenByDescending(x => x.ValidFrom)
     .FirstOrDefaultAsync();
    }
    public async Task<List<TestTariff>> GetForBulkAdjustmentAsync(
    Guid organizationTypeId,
    Guid? customerId,
    DateTime effectiveDate,
    List<Guid>? testIds,
    List<Guid>? testPanelIds,
    bool applyToAll)
    {
        if (organizationTypeId == Guid.Empty)
            return new List<TestTariff>();

        var query = _context.TestTariffs
            .Where(x =>
                !x.IsDeleted &&
                x.IsActive &&
                x.OrganizationTypeId == organizationTypeId &&
                x.ValidFrom <= effectiveDate &&
                (!x.ValidTo.HasValue || x.ValidTo.Value >= effectiveDate) &&
                x.CustomerId == customerId);

        if (!applyToAll)
        {
            var hasTests = testIds != null && testIds.Count > 0;
            var hasPanels = testPanelIds != null && testPanelIds.Count > 0;

            if (!hasTests && !hasPanels)
                return new List<TestTariff>();

            query = query.Where(x =>
                (hasTests && x.TestId.HasValue && testIds!.Contains(x.TestId.Value)) ||
                (hasPanels && x.TestPanelId.HasValue && testPanelIds!.Contains(x.TestPanelId.Value)));
        }

        return await query
            .Include(x => x.Test)
            .Include(x => x.TestPanel)
            .Include(x => x.OrganizationType)
            .Include(x => x.Customer)
            .Include(x => x.StandardSample)
            .Include(x => x.Matrix)
            .OrderBy(x => x.TestId.HasValue ? 0 : 1)
            .ThenBy(x => x.Test != null ? x.Test.Name : x.TestPanel!.Name)
            .ThenBy(x => x.Priority)
            .ThenByDescending(x => x.ValidFrom)
            .ToListAsync();
    }

    public async Task BulkAdjustAsync(
        List<TestTariff> tariffsToClose,
        List<TestTariff> newTariffs)
    {
        if (tariffsToClose != null)
        {
            foreach (var tariff in tariffsToClose)
            {
                Update(tariff);
            }
        }

        if (newTariffs != null)
        {
            foreach (var tariff in newTariffs)
            {
                await AddAsync(tariff);
            }
        }
    }
    public async Task AddAsync(TestTariff tariff)
    {
        await _context.TestTariffs.AddAsync(tariff);
    }

    public void Update(TestTariff tariff)
    {
        _context.TestTariffs.Update(tariff);
    }

    public void Delete(TestTariff tariff)
    {
        _context.TestTariffs.Remove(tariff);
    }
}