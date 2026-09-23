using Barman.Domain.Entities;

namespace Barman.Application.Interfaces;

public interface ITestAssignmentWorkflowService
{
    Task AssignToSectionAsync(
        Guid assignmentId,
        Guid departmentId);

    Task UpdateAssignmentDepartmentAsync(
    Guid assignmentId,
    Guid departmentId);

    Task AssignPanelToSectionAsync(
        Guid sampleId,
        Guid testPanelId,
        Guid departmentId);

    Task AssignToAnalystAsync(
        Guid assignmentId,
        Guid analystId);

    Task SubmitResultAsync(
        Guid assignmentId,
        string result);

    Task SubmitResultSetAsync(Guid assignmentId);

    Task SubmitSelectedAssignmentsToSectionAsync(
    List<Guid> assignmentIds);

    Task ReturnSelectedAssignmentsToTechnicalManagerAsync(
    List<Guid> assignmentIds);
    Task ApproveBySectionAsync(
        Guid assignmentId);

    Task RejectBySectionAsync(
        Guid assignmentId,
        string? reason);

    Task ApprovePanelBySectionAsync(
        Guid sampleId,
        Guid testPanelId);

    Task RejectPanelBySectionAsync(
        Guid sampleId,
        Guid testPanelId,
        string? reason);

    Task ApproveByTechnicalManagerAsync(
        Guid assignmentId);

    Task RejectByTechnicalManagerAsync(
        Guid assignmentId,
        string? reason);

    Task ApproveByDirectorAsync(
        Guid assignmentId);

    Task RejectByDirectorAsync(
        Guid assignmentId,
        string? reason);

    Task RequestReceptionCorrectionAsync(
    Guid receptionId,
    Guid sampleId,
    string reason);
}
