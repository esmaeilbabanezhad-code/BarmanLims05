using Barman.Application.DTOs.TechnicalManager;
using Barman.Application.Interfaces;
using Barman.Domain.Entities;

namespace Barman.Application.Services;

public class TechnicalManagerRoutingService
    : ITechnicalManagerRoutingService
{
    private readonly IUnitOfWork _unitOfWork;

    public TechnicalManagerRoutingService(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TechnicalManagerRoutingResultDto?> ResolveAsync(
        Guid customerId,
        Guid? sampleCategoryId,
        Guid? matrixId,
        Guid? testPanelId,
        Guid testId,
        Guid? departmentId,
        CancellationToken cancellationToken = default)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("Customer is required.");

        if (testId == Guid.Empty)
            throw new ArgumentException("Test is required.");

        var rules =
            await _unitOfWork.TechnicalManagerRoutingRules
                .GetApplicableAsync(
                    customerId,
                    sampleCategoryId,
                    matrixId,
                    testPanelId,
                    testId,
                    departmentId);

        if (rules.Count == 0)
            return null;

        var bestRule = rules
            .OrderByDescending(x => x.CustomerId.HasValue)
            .ThenByDescending(x => x.SampleCategoryId.HasValue)
            .ThenByDescending(x => x.MatrixId.HasValue)
            .ThenByDescending(x => x.TestPanelId.HasValue)
            .ThenByDescending(x => x.TestId.HasValue)
            .ThenByDescending(x => x.DepartmentId.HasValue)
            .ThenBy(x => x.Priority)
            .ThenBy(x => x.Name)
            .First();

        var scopeMembers =
            await _unitOfWork.EmployeeTechnicalManagerScopes
                .GetByScopeIdAsync(
                    bestRule.TechnicalManagerScopeId);

        var activeMembers = scopeMembers
            .Where(x =>
                !x.IsDeleted &&
                x.Employee != null &&
                x.Employee.IsSystemUser)
            .ToList();

        if (activeMembers.Count == 0)
            throw new InvalidOperationException(
                $"برای محدوده مسئول فنی '{bestRule.TechnicalManagerScope?.Name}' هیچ مسئول فنی فعالی تعریف نشده است.");

        var primaryMembers = activeMembers
            .Where(x => x.IsPrimary)
            .ToList();

        EmployeeTechnicalManagerScope selectedMember;

        if (primaryMembers.Count == 1)
        {
            selectedMember = primaryMembers[0];
        }
        else if (primaryMembers.Count > 1)
        {
            throw new InvalidOperationException(
                $"برای محدوده مسئول فنی '{bestRule.TechnicalManagerScope?.Name}' بیش از یک مسئول فنی اصلی تعریف شده است.");
        }
        else if (activeMembers.Count == 1)
        {
            selectedMember = activeMembers[0];
        }
        else
        {
            throw new InvalidOperationException(
                $"برای محدوده مسئول فنی '{bestRule.TechnicalManagerScope?.Name}' چند مسئول فنی تعریف شده ولی هیچ‌کدام به عنوان اصلی مشخص نشده‌اند.");
        }

        return new TechnicalManagerRoutingResultDto
        {
            TechnicalManagerId = selectedMember.EmployeeId,
            TechnicalManagerScopeId = bestRule.TechnicalManagerScopeId,
            DepartmentId = bestRule.DepartmentId ?? departmentId,
            RoutingRuleId = bestRule.Id
        };
    }
}