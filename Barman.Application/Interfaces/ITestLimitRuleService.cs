using Barman.Domain.Entities;

namespace Barman.Application.Interfaces;

public interface ITestLimitRuleService
{
    Task<List<TestLimitRule>> GetByTestIdAsync(
        Guid testId,
        CancellationToken cancellationToken = default);

    Task<TestLimitRule?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<TestLimitRule> CreateAsync(
        TestLimitRule rule,
        CancellationToken cancellationToken = default);

    Task<TestLimitRule?> UpdateAsync(
        TestLimitRule rule,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}