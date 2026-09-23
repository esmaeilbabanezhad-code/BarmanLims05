using Barman.Domain.Entities.Reporting;

namespace Barman.Application.Interfaces.Repositories.Reporting;

public interface IReportTemplateSectionRepository
{
    Task<List<ReportTemplateSection>> GetByTemplateIdAsync(
        Guid templateId,
        CancellationToken cancellationToken = default);

    Task<ReportTemplateSection?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        ReportTemplateSection entity,
        CancellationToken cancellationToken = default);

    void Update(ReportTemplateSection entity);

    void Delete(ReportTemplateSection entity);
}
