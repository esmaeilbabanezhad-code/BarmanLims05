using Barman.Application.Interfaces.Repositories.Reporting;
using Barman.Domain.Entities.Reporting;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories.Reporting;

public class IssuedReportRepository : IIssuedReportRepository
{
    private readonly ApplicationDbContext _context;

    public IssuedReportRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(IssuedReport report)
    {
        await _context.IssuedReports.AddAsync(report);
    }

    public async Task<IssuedReport?> GetByIdAsync(Guid id)
    {
        return await _context.IssuedReports
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IssuedReport?> GetCurrentBySampleIdAsync(
        Guid sampleId)
    {
        if (sampleId == Guid.Empty)
            return null;

        return await _context.IssuedReports
            .Where(x =>
                x.SampleId == sampleId &&
                x.IsCurrent &&
                !x.IsCancelled &&
                !x.IsDeleted)
            .OrderByDescending(x => x.Version)
            .FirstOrDefaultAsync();
    }

    public async Task<List<IssuedReport>> GetBySampleIdAsync(
        Guid sampleId)
    {
        if (sampleId == Guid.Empty)
            return new List<IssuedReport>();

        return await _context.IssuedReports
            .Where(x =>
                x.SampleId == sampleId &&
                !x.IsDeleted)
            .OrderByDescending(x => x.Version)
            .ToListAsync();
    }

    public async Task<int> GetNextVersionAsync(
        Guid sampleId)
    {
        if (sampleId == Guid.Empty)
            return 1;

        var maxVersion =
            await _context.IssuedReports
                .Where(x =>
                    x.SampleId == sampleId &&
                    !x.IsDeleted)
                .Select(x => (int?)x.Version)
                .MaxAsync();

        return (maxVersion ?? 0) + 1;
    }
    public async Task<List<IssuedReport>> GetByYearAsync(int year)
    {
        var start = new DateTime(year, 1, 1);
        var end = start.AddYears(1);

        return await _context.IssuedReports
            .Where(x =>
                x.IssuedAt >= start &&
                x.IssuedAt < end &&
                !x.IsDeleted)
            .ToListAsync();
    }
    public void Update(IssuedReport report)
    {
        _context.IssuedReports.Update(report);
    }
}
