using Barman.Application.DTOs.Reception;
using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface ITestAssignmentRepository
{
    Task AddAsync(TestAssignment assignment);

    Task<TestAssignment?> GetByIdAsync(Guid id);

    Task<List<TestAssignment>> GetBySampleIdAsync(Guid sampleId);

    Task<List<TestAssignment>> GetBySampleIdAndTestPanelIdAsync(
        Guid sampleId,
        Guid testPanelId);

    Task<List<TestAssignment>> GetByReceptionIdAsync(Guid receptionId);

    Task<List<TestAssignment>> GetForFinalReportByReceptionIdAsync(
    Guid receptionId);

    Task<List<TestAssignment>> GetForFinalReportBySampleIdAsync(
    Guid sampleId);

    Task<List<ReceptionTestStatusDto>> GetReceptionTestStatusesAsync(
    Guid receptionId);

    Task<List<TestAssignment>> GetPendingForTechnicalManagerAsync(Guid technicalManagerId);

    Task<List<TestAssignment>> GetPendingForSectionHeadAsync(Guid departmentId);

    Task<List<TestAssignment>> GetPendingResultApprovalForSectionHeadAsync(
    Guid departmentId);

    Task<List<TestAssignment>> GetPendingResultApprovalForTechnicalManagerAsync();

    Task<List<TestAssignment>> GetPendingResultApprovalForDirectorAsync();
    Task<List<TestAssignment>> GetPendingForAnalystAsync(Guid analystId);

    Task<List<TestAssignment>> GetPendingForAnalystBySampleAsync(
    Guid analystId,
    Guid sampleId);
  
    void Update(TestAssignment assignment);

    void Delete(TestAssignment assignment);
}