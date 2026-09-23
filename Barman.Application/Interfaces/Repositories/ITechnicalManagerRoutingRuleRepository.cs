using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface ITechnicalManagerRoutingRuleRepository
{
    Task<List<TechnicalManagerRoutingRule>> GetAllAsync();

    Task<TechnicalManagerRoutingRule?> GetByIdAsync(Guid id);

    Task<List<TechnicalManagerRoutingRule>> GetApplicableAsync(
        Guid? customerId,
        Guid? sampleCategoryId,
        Guid? matrixId,
        Guid? testPanelId,
        Guid? testId,
        Guid? departmentId);

    Task<bool> ExistsByCodeAsync(
        string code,
        Guid? excludeId = null);

    Task AddAsync(
        TechnicalManagerRoutingRule rule);

    void Update(
        TechnicalManagerRoutingRule rule);

    void Delete(
        TechnicalManagerRoutingRule rule);
}