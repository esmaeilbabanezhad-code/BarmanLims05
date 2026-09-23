using Barman.Domain.Entities.Reporting;

namespace Barman.Application.Interfaces.Repositories.Reporting;

public interface IIssuedReportRepository
{
    Task AddAsync(IssuedReport report);

    Task<IssuedReport?> GetByIdAsync(Guid id);

    Task<IssuedReport?> GetCurrentBySampleIdAsync(Guid sampleId);

    Task<List<IssuedReport>> GetBySampleIdAsync(Guid sampleId);

    Task<int> GetNextVersionAsync(Guid sampleId);

    Task<List<IssuedReport>> GetByYearAsync(int year);

    void Update(IssuedReport report);
}
