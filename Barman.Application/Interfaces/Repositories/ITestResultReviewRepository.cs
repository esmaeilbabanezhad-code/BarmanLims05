using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface ITestResultReviewRepository
{
    Task AddAsync(TestResultReview review);

    Task<TestResultReview?> GetByIdAsync(Guid id);

    Task<List<TestResultReview>> GetByAssignmentIdAsync(
        Guid testAssignmentId);

    Task<List<TestResultReview>> GetByReviewerEmployeeIdAsync(
        Guid reviewerEmployeeId);

    void Update(TestResultReview review);

    void Delete(TestResultReview review);
}
