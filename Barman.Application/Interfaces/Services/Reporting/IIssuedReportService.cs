using Barman.Application.DTOs.Reporting;

namespace Barman.Application.Interfaces.Services.Reporting;

public interface IIssuedReportService
{
    Task<Guid?> IssueAsync(
        Guid sampleId,
        Guid reportTemplateId,
        string templateCode,
        string templateVersion,
        Guid? issuedByEmployeeId = null,
        CancellationToken cancellationToken = default);

    Task<FinalReportData?> GetSnapshotAsync(
        Guid issuedReportId,
        CancellationToken cancellationToken = default);

    Task<List<(
        Guid Id,
        int Version,
        string ReportNumber,
        DateTime IssuedAt,
        bool IsCurrent,
        bool IsCancelled)>>
        GetVersionsAsync(
            Guid sampleId,
            CancellationToken cancellationToken = default);

    Task<bool> CancelAsync(
    Guid issuedReportId,
    string cancellationReason,
    Guid? cancelledByEmployeeId = null,
    CancellationToken cancellationToken = default);

    Task<bool> CanIssueAsync(
        Guid sampleId,
        CancellationToken cancellationToken = default);
}