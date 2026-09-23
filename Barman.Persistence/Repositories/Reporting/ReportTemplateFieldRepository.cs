using Barman.Application.Interfaces.Repositories.Reporting;
using Barman.Domain.Entities.Reporting;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories.Reporting;

public class ReportTemplateFieldRepository : IReportTemplateFieldRepository
{
    private readonly ApplicationDbContext _context;

    public ReportTemplateFieldRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ReportTemplateField>> GetBySectionIdAsync(
        Guid sectionId,
        CancellationToken cancellationToken = default)
    {
        return await _context.ReportTemplateFields
            .Where(x => x.ReportTemplateSectionId == sectionId)
            .AsNoTracking()
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<ReportTemplateField?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.ReportTemplateFields
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddAsync(
        ReportTemplateField entity,
        CancellationToken cancellationToken = default)
    {
        await _context.ReportTemplateFields.AddAsync(
            entity,
            cancellationToken);
    }

    public void Update(ReportTemplateField entity)
    {
        _context.ReportTemplateFields.Update(entity);
    }

    public void Delete(ReportTemplateField entity)
    {
        _context.ReportTemplateFields.Remove(entity);
    }
}
