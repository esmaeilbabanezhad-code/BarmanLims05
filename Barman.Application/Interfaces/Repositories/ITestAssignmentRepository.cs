using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface ITestAssignmentRepository
{
    Task AddAsync(TestAssignment assignment);

    Task<TestAssignment?> GetByIdAsync(Guid id);

    Task<List<TestAssignment>> GetBySampleIdAsync(Guid sampleId);

    Task<List<TestAssignment>> GetByReceptionIdAsync(Guid receptionId);

    Task<List<TestAssignment>> GetPendingForTechnicalManagerAsync();

    Task<List<TestAssignment>> GetPendingForSectionHeadAsync(Guid departmentId);

    Task<List<TestAssignment>> GetPendingResultApprovalForSectionHeadAsync(
    Guid departmentId);

    Task<List<TestAssignment>> GetPendingResultApprovalForTechnicalManagerAsync();

    Task<List<TestAssignment>> GetPendingResultApprovalForDirectorAsync();
    Task<List<TestAssignment>> GetPendingForAnalystAsync(Guid analystId);

    Task AssignToAnalystAsync(
    List<Guid> assignmentIds,
    Guid analystId);
    void Update(TestAssignment assignment);

    void Delete(TestAssignment assignment);
}