using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface ITestLimitRuleRepository
{
    Task<TestLimitRule?> GetByIdAsync(Guid id);
    Task<List<TestLimitRule>> GetByTestIdAsync(Guid testId);
    Task<List<TestLimitRule>> GetApplicableAsync(
    Guid testId,
    Guid? customerId,
    Guid? matrixId,
    Guid? sampleCategoryId);

    Task AddAsync(TestLimitRule rule);

    void Update(TestLimitRule rule);

    void Delete(TestLimitRule rule);
}
