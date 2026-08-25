using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface ITestLimitChangeRequestRepository
{
    Task AddAsync(TestLimitChangeRequest request);

    Task<TestLimitChangeRequest?> GetByIdAsync(Guid id);

    Task<List<TestLimitChangeRequest>> GetPendingForSectionHeadAsync(
        Guid departmentId);

    void Update(TestLimitChangeRequest request);
}