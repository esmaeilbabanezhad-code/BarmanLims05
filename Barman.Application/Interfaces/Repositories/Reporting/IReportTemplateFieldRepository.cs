using Barman.Domain.Entities.Reporting;

namespace Barman.Application.Interfaces.Repositories.Reporting;

public interface IReportTemplateFieldRepository
{
    Task<List<ReportTemplateField>> GetBySectionIdAsync(
        Guid sectionId,
        CancellationToken cancellationToken = default);

    Task<ReportTemplateField?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        ReportTemplateField entity,
        CancellationToken cancellationToken = default);

    void Update(ReportTemplateField entity);

    void Delete(ReportTemplateField entity);
}
