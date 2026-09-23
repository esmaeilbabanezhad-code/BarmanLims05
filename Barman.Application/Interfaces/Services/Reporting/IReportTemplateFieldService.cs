using Barman.Domain.Entities.Reporting;

namespace Barman.Application.Interfaces.Services.Reporting;

public interface IReportTemplateFieldService
{
    Task<List<ReportTemplateField>> GetBySectionIdAsync(
        Guid sectionId,
        CancellationToken cancellationToken = default);

    Task<ReportTemplateField> CreateAsync(
        ReportTemplateField entity,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        ReportTemplateField entity,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
