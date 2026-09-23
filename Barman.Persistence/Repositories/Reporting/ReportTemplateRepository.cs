using Barman.Application.Interfaces.Repositories.Reporting;
using Barman.Domain.Entities.Reporting;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories.Reporting;

public class ReportTemplateRepository : IReportTemplateRepository
{
    private readonly ApplicationDbContext _context;

    public ReportTemplateRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ReportTemplate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.ReportTemplates
            .Include(x => x.Sections)
            .ThenInclude(x => x.Fields)
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<ReportTemplate?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.ReportTemplates
            .Include(x => x.Sections)
            .ThenInclude(x => x.Fields)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<ReportTemplate?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return await _context.ReportTemplates
            .Include(x => x.Sections)
            .ThenInclude(x => x.Fields)
            .FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
    }

    public async Task AddAsync(
        ReportTemplate entity,
        CancellationToken cancellationToken = default)
    {
        await _context.ReportTemplates.AddAsync(entity, cancellationToken);
    }

    public void Update(ReportTemplate entity)
    {
        _context.ReportTemplates.Update(entity);
    }

    public void Delete(ReportTemplate entity)
    {
        _context.ReportTemplates.Remove(entity);
    }

    public async Task<ReportTemplate?> GetWithSectionsAsync(
    Guid id,
    CancellationToken cancellationToken = default)
    {
        return await _context.ReportTemplates
            .Include(x => x.Sections)
            .ThenInclude(x => x.Fields)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

}
