using Barman.Application.Interfaces.Repositories.Reporting;
using Barman.Domain.Entities.Reporting;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories.Reporting;

public class ReportTemplateSectionRepository : IReportTemplateSectionRepository
{
    private readonly ApplicationDbContext _context;

    public ReportTemplateSectionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ReportTemplateSection>> GetByTemplateIdAsync(
        Guid templateId,
        CancellationToken cancellationToken = default)
    {
        return await _context.ReportTemplateSections
            .Where(x => x.ReportTemplateId == templateId)
            .Include(x => x.Fields)
            .AsNoTracking()
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<ReportTemplateSection?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.ReportTemplateSections
            .Include(x => x.Fields)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddAsync(
        ReportTemplateSection entity,
        CancellationToken cancellationToken = default)
    {
        await _context.ReportTemplateSections.AddAsync(
            entity,
            cancellationToken);
    }

    public void Update(ReportTemplateSection entity)
    {
        _context.ReportTemplateSections.Update(entity);
    }

    public void Delete(ReportTemplateSection entity)
    {
        _context.ReportTemplateSections.Remove(entity);
    }
}
