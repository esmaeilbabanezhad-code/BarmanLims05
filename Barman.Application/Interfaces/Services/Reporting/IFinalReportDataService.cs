using Barman.Application.DTOs.Reporting;

namespace Barman.Application.Interfaces.Services.Reporting;

public interface IFinalReportDataService
{
    Task<FinalReportData?> GetByReceptionIdAsync(
        Guid receptionId,
        CancellationToken cancellationToken = default);

    Task<FinalReportData?> GetBySampleIdAsync(
    Guid sampleId,
    CancellationToken cancellationToken = default);

}

