using System.Text.Json;
using Barman.Application.DTOs.Reporting;
using Barman.Application.Interfaces;
using Barman.Application.Interfaces.Services.Reporting;
using Barman.Domain.Enums;

namespace Barman.Application.Services.Reporting;

public class IssuedReportService : IIssuedReportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFinalReportDataService _finalReportDataService;
    private readonly ICurrentUserService _currentUserService;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public IssuedReportService(
        IUnitOfWork unitOfWork,
        IFinalReportDataService finalReportDataService,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _finalReportDataService = finalReportDataService;
        _currentUserService = currentUserService;
    }

    public async Task<bool> CanIssueAsync(
        Guid sampleId,
        CancellationToken cancellationToken = default)
    {
        if (sampleId == Guid.Empty)
            return false;

        var assignments =
            await _unitOfWork.TestAssignments
                .GetBySampleIdAsync(sampleId);

        if (assignments is null ||
            assignments.Count == 0)
        {
            return false;
        }

        return assignments.All(x =>
            x.IsApprovedBySection &&
            x.IsApprovedByTechManager &&
            x.IsApprovedByDirector &&
            x.WorkflowStage ==
                TestAssignmentWorkflowStage.Completed);
    }

    public async Task<Guid?> IssueAsync(
        Guid sampleId,
        Guid reportTemplateId,
        string templateCode,
        string templateVersion,
        Guid? issuedByEmployeeId = null,
        CancellationToken cancellationToken = default)
    {
        if (sampleId == Guid.Empty)
            return null;

        if (reportTemplateId == Guid.Empty)
            return null;

        if (string.IsNullOrWhiteSpace(templateCode))
            return null;

        if (string.IsNullOrWhiteSpace(templateVersion))
            return null;

        var assignments =
            await _unitOfWork.TestAssignments
                .GetBySampleIdAsync(sampleId);

        if (assignments is null ||
            assignments.Count == 0)
        {
            return null;
        }

        var allAssignmentsApproved =
            assignments.All(x =>
                x.IsApprovedBySection &&
                x.IsApprovedByTechManager &&
                x.IsApprovedByDirector &&
                x.WorkflowStage ==
                    TestAssignmentWorkflowStage.Completed);

        if (!allAssignmentsApproved)
        {
            return null;
        }

        var reportData =
            await _finalReportDataService.GetBySampleIdAsync(
                sampleId,
                cancellationToken);

        if (reportData is null)
            return null;

        var sampleIdFromReport =
            reportData.Sample.Id;

        if (sampleIdFromReport == Guid.Empty ||
            sampleIdFromReport != sampleId)
        {
            return null;
        }

        var currentEmployeeId =
            _currentUserService.EmployeeId;

        if (!currentEmployeeId.HasValue ||
            currentEmployeeId.Value == Guid.Empty)
        {
            return null;
        }

        var existingCurrent =
            await _unitOfWork.IssuedReports
                .GetCurrentBySampleIdAsync(sampleId);

        if (existingCurrent is not null)
        {
            existingCurrent.IsCurrent = false;

            _unitOfWork.IssuedReports
                .Update(existingCurrent);
        }

        var version =
            await _unitOfWork.IssuedReports
                .GetNextVersionAsync(sampleId);

        reportData.ReportVersion =
            version.ToString();

        reportData.TemplateCode =
            templateCode;

        reportData.TemplateVersion =
            templateVersion;

        reportData.IssueDate =
            DateTime.Now;

        var snapshotJson =
            JsonSerializer.Serialize(
                reportData,
                JsonOptions);

        var report =
            new Barman.Domain.Entities.Reporting.IssuedReport
            {
                Id = Guid.NewGuid(),

                ReportNumber =
                    reportData.ReportNumber,

                Version =
                    version,

                ReceptionId =
                    reportData.ReceptionId,

                SampleId =
                    sampleId,

                ReportTemplateId =
                    reportTemplateId,

                TemplateCode =
                    templateCode,

                TemplateVersion =
                    templateVersion,

                SnapshotJson =
                    snapshotJson,

                IssuedAt =
                    DateTime.UtcNow,

                IssuedByEmployeeId =
                    currentEmployeeId.Value,

                IsCurrent =
                    true,

                IsCancelled =
                    false,

                IsDeleted =
                    false,

                IsActive =
                    true
            };

        await _unitOfWork.IssuedReports
            .AddAsync(report);

        await _unitOfWork
            .SaveChangesAsync(cancellationToken);

        return report.Id;
    }

    public async Task<bool> CancelAsync(
        Guid issuedReportId,
        string cancellationReason,
        Guid? cancelledByEmployeeId = null,
        CancellationToken cancellationToken = default)
    {
        if (issuedReportId == Guid.Empty)
            return false;

        if (string.IsNullOrWhiteSpace(cancellationReason))
            return false;

        var report =
            await _unitOfWork.IssuedReports
                .GetByIdAsync(issuedReportId);

        if (report is null ||
            report.IsDeleted ||
            report.IsCancelled)
        {
            return false;
        }

        var currentEmployeeId =
            _currentUserService.EmployeeId;

        if (!currentEmployeeId.HasValue ||
            currentEmployeeId.Value == Guid.Empty)
        {
            return false;
        }

        report.IsCancelled = true;
        report.IsCurrent = false;
        report.CancelledAt = DateTime.UtcNow;
        report.CancelledByEmployeeId =
            currentEmployeeId.Value;
        report.CancellationReason =
            cancellationReason.Trim();

        _unitOfWork.IssuedReports
            .Update(report);

        await _unitOfWork
            .SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<FinalReportData?> GetSnapshotAsync(
        Guid issuedReportId,
        CancellationToken cancellationToken = default)
    {
        if (issuedReportId == Guid.Empty)
            return null;

        var report =
            await _unitOfWork.IssuedReports
                .GetByIdAsync(issuedReportId);

        if (report is null ||
            report.IsDeleted ||
            string.IsNullOrWhiteSpace(
                report.SnapshotJson))
        {
            return null;
        }

        return JsonSerializer.Deserialize<FinalReportData>(
            report.SnapshotJson,
            JsonOptions);
    }

    public async Task<List<(
        Guid Id,
        int Version,
        string ReportNumber,
        DateTime IssuedAt,
        bool IsCurrent,
        bool IsCancelled)>>
        GetVersionsAsync(
            Guid sampleId,
            CancellationToken cancellationToken = default)
    {
        if (sampleId == Guid.Empty)
        {
            return new List<
                (
                    Guid Id,
                    int Version,
                    string ReportNumber,
                    DateTime IssuedAt,
                    bool IsCurrent,
                    bool IsCancelled
                )>();
        }

        var reports =
            await _unitOfWork.IssuedReports
                .GetBySampleIdAsync(sampleId);

        return reports
            .Select(x =>
                (
                    x.Id,
                    x.Version,
                    x.ReportNumber,
                    x.IssuedAt,
                    x.IsCurrent,
                    x.IsCancelled
                ))
            .ToList();
    }
}
