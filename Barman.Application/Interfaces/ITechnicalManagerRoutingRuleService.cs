using Barman.Domain.Entities;

namespace Barman.Application.Interfaces;

public interface ITechnicalManagerRoutingRuleService
{
    Task<List<TechnicalManagerRoutingRule>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<TechnicalManagerRoutingRule?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<List<TechnicalManagerRoutingRule>> GetApplicableAsync(
        Guid? customerId,
        Guid? sampleCategoryId,
        Guid? matrixId,
        Guid? testPanelId,
        Guid? testId,
        Guid? departmentId,
        CancellationToken cancellationToken = default);

    Task<TechnicalManagerRoutingRule> AddAsync(
        TechnicalManagerRoutingRule rule,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        TechnicalManagerRoutingRule rule,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ActivateAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}