using Barman.Application.Interfaces;
using Barman.Domain.Entities;

namespace Barman.Application.Services;

public class TestLimitRuleService : ITestLimitRuleService
{
    private readonly IUnitOfWork _unitOfWork;

    public TestLimitRuleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TestLimitRule>> GetByTestIdAsync(
        Guid testId,
        CancellationToken cancellationToken = default)
    {
        if (testId == Guid.Empty)
            return new List<TestLimitRule>();

        return await _unitOfWork.TestLimitRules
            .GetByTestIdAsync(testId);
    }

    public async Task<TestLimitRule?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return null;

        return await _unitOfWork.TestLimitRules
            .GetByIdAsync(id);
    }

    public async Task<TestLimitRule> CreateAsync(
        TestLimitRule rule,
        CancellationToken cancellationToken = default)
    {
        if (rule is null)
            throw new ArgumentNullException(nameof(rule));

        if (rule.TestId == Guid.Empty)
            throw new InvalidOperationException(
                "آزمون مشخص نشده است.");

        if (rule.LimitReferenceId == Guid.Empty)
            throw new InvalidOperationException(
                "مرجع حد مجاز مشخص نشده است.");

        await _unitOfWork.TestLimitRules.AddAsync(rule);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return rule;
    }

    public async Task<TestLimitRule?> UpdateAsync(
        TestLimitRule rule,
        CancellationToken cancellationToken = default)
    {
        if (rule is null)
            return null;

        if (rule.Id == Guid.Empty)
            return null;

        _unitOfWork.TestLimitRules.Update(rule);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return rule;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return false;

        var rule = await _unitOfWork.TestLimitRules
            .GetByIdAsync(id);

        if (rule is null)
            return false;

        _unitOfWork.TestLimitRules.Delete(rule);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
