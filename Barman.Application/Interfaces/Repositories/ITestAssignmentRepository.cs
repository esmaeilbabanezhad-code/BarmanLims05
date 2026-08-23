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

    void Update(TestAssignment assignment);

    void Delete(TestAssignment assignment);
}