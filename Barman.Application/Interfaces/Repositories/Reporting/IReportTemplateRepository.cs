using Barman.Domain.Entities.Reporting;

namespace Barman.Application.Interfaces.Repositories.Reporting;

public interface IReportTemplateRepository
{
    Task<List<ReportTemplate>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<ReportTemplate?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ReportTemplate?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        ReportTemplate entity,
        CancellationToken cancellationToken = default);

    Task<ReportTemplate?> GetWithSectionsAsync(
    Guid id,
    CancellationToken cancellationToken = default);

    void Update(ReportTemplate entity);

    void Delete(ReportTemplate entity);
}
