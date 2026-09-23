using Barman.Domain.Entities.Reporting;

namespace Barman.Application.Interfaces.Services.Reporting;

public interface IReportTemplateService
{
    Task<List<ReportTemplate>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<ReportTemplate?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ReportTemplate?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<ReportTemplate> CreateAsync(
        ReportTemplate entity,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        ReportTemplate entity,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ReportTemplate?> CloneAsync(
    Guid templateId,
    string newCode,
    string newName,
    CancellationToken cancellationToken = default);

}
