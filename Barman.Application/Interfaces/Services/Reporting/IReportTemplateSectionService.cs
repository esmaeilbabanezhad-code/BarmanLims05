using Barman.Domain.Entities.Reporting;

namespace Barman.Application.Interfaces.Services.Reporting;

public interface IReportTemplateSectionService
{
    Task<List<ReportTemplateSection>> GetByTemplateIdAsync(
        Guid templateId,
        CancellationToken cancellationToken = default);

    Task<ReportTemplateSection> CreateAsync(
        ReportTemplateSection entity,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        ReportTemplateSection entity,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
