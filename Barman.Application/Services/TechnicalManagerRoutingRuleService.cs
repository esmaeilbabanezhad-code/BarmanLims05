using Barman.Application.Interfaces;
using Barman.Domain.Entities;

namespace Barman.Application.Services;

public class TechnicalManagerRoutingRuleService
    : ITechnicalManagerRoutingRuleService
{
    private readonly IUnitOfWork _unitOfWork;

    public TechnicalManagerRoutingRuleService(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TechnicalManagerRoutingRule>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.TechnicalManagerRoutingRules
            .GetAllAsync();
    }

    public async Task<TechnicalManagerRoutingRule?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.TechnicalManagerRoutingRules
            .GetByIdAsync(id);
    }

    public async Task<List<TechnicalManagerRoutingRule>> GetApplicableAsync(
        Guid? customerId,
        Guid? sampleCategoryId,
        Guid? matrixId,
        Guid? testPanelId,
        Guid? testId,
        Guid? departmentId,
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.TechnicalManagerRoutingRules
            .GetApplicableAsync(
                customerId,
                sampleCategoryId,
                matrixId,
                testPanelId,
                testId,
                departmentId);
    }

    public async Task<TechnicalManagerRoutingRule> AddAsync(
        TechnicalManagerRoutingRule rule,
        CancellationToken cancellationToken = default)
    {
        if (rule == null)
            throw new ArgumentNullException(nameof(rule));

        rule.Code = rule.Code?.Trim() ?? "";
        rule.Name = rule.Name?.Trim() ?? "";
        

        if (string.IsNullOrWhiteSpace(rule.Code))
            throw new InvalidOperationException(
                "کد قانون مسیریابی الزامی است.");

        if (string.IsNullOrWhiteSpace(rule.Name))
            throw new InvalidOperationException(
                "نام قانون مسیریابی الزامی است.");

        if (rule.Priority < 0)
            throw new InvalidOperationException(
                "اولویت نمی‌تواند منفی باشد.");

        if (rule.TechnicalManagerScopeId == Guid.Empty)
            throw new InvalidOperationException(
                "محدوده مسئول فنی الزامی است.");

        var duplicate =
            await _unitOfWork.TechnicalManagerRoutingRules
                .ExistsByCodeAsync(rule.Code);

        if (duplicate)
            throw new InvalidOperationException(
                "کد قانون مسیریابی تکراری است.");

        rule.IsActive = true;
        rule.IsDeleted = false;

        await _unitOfWork.TechnicalManagerRoutingRules
            .AddAsync(rule);

        await _unitOfWork.SaveChangesAsync();

        return rule;
    }

    public async Task<bool> UpdateAsync(
        TechnicalManagerRoutingRule rule,
        CancellationToken cancellationToken = default)
    {
        if (rule == null)
            throw new ArgumentNullException(nameof(rule));

        if (rule.Id == Guid.Empty)
            throw new InvalidOperationException(
                "شناسه قانون مسیریابی نامعتبر است.");

        rule.Code = rule.Code?.Trim() ?? "";
        rule.Name = rule.Name?.Trim() ?? "";
        

        if (string.IsNullOrWhiteSpace(rule.Code))
            throw new InvalidOperationException(
                "کد قانون مسیریابی الزامی است.");

        if (string.IsNullOrWhiteSpace(rule.Name))
            throw new InvalidOperationException(
                "نام قانون مسیریابی الزامی است.");

        if (rule.Priority < 0)
            throw new InvalidOperationException(
                "اولویت نمی‌تواند منفی باشد.");

        if (rule.TechnicalManagerScopeId == Guid.Empty)
            throw new InvalidOperationException(
                "محدوده مسئول فنی الزامی است.");

        var existing =
            await _unitOfWork.TechnicalManagerRoutingRules
                .GetByIdAsync(rule.Id);

        if (existing == null)
            return false;

        var duplicate =
            await _unitOfWork.TechnicalManagerRoutingRules
                .ExistsByCodeAsync(
                    rule.Code,
                    rule.Id);

        if (duplicate)
            throw new InvalidOperationException(
                "کد قانون مسیریابی تکراری است.");

        existing.Code = rule.Code;
        existing.Name = rule.Name;
        

        existing.CustomerId = rule.CustomerId;
        existing.SampleCategoryId = rule.SampleCategoryId;
        existing.MatrixId = rule.MatrixId;
        existing.TestPanelId = rule.TestPanelId;
        existing.TestId = rule.TestId;
        existing.DepartmentId = rule.DepartmentId;

        existing.TechnicalManagerScopeId =
            rule.TechnicalManagerScopeId;

        existing.Priority = rule.Priority;
        existing.IsActive = rule.IsActive;
        existing.ModifiedAt = DateTimeOffset.UtcNow;

        _unitOfWork.TechnicalManagerRoutingRules
            .Update(existing);

        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var rule =
            await _unitOfWork.TechnicalManagerRoutingRules
                .GetByIdAsync(id);

        if (rule == null)
            return false;

        rule.IsDeleted = true;
        rule.IsActive = false;
        rule.ModifiedAt = DateTimeOffset.UtcNow;

        _unitOfWork.TechnicalManagerRoutingRules
            .Update(rule);

        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ActivateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var rule =
            await _unitOfWork.TechnicalManagerRoutingRules
                .GetByIdAsync(id);

        if (rule == null)
            return false;

        rule.IsDeleted = false;
        rule.IsActive = true;
        rule.ModifiedAt = DateTimeOffset.UtcNow;

        _unitOfWork.TechnicalManagerRoutingRules
            .Update(rule);

        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var rule =
            await _unitOfWork.TechnicalManagerRoutingRules
                .GetByIdAsync(id);

        if (rule == null)
            return false;

        rule.IsActive = false;
        rule.ModifiedAt = DateTimeOffset.UtcNow;

        _unitOfWork.TechnicalManagerRoutingRules
            .Update(rule);

        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}