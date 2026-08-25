using Barman.Application.Interfaces;
using Barman.Domain.Entities;

namespace Barman.Application.Services;

public class TestAssignmentService
{
    private readonly IUnitOfWork _unitOfWork;

    public TestAssignmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TestAssignment>> GetBySampleIdAsync(Guid sampleId)
    {
        if (sampleId == Guid.Empty)
            return new List<TestAssignment>();

        return await _unitOfWork.TestAssignments
            .GetBySampleIdAsync(sampleId);
    }

    public async Task<List<TestAssignment>> GetByReceptionIdAsync(Guid receptionId)
    {
        if (receptionId == Guid.Empty)
            return new List<TestAssignment>();

        return await _unitOfWork.TestAssignments
            .GetByReceptionIdAsync(receptionId);
    }

    public async Task<List<Department>> GetDepartmentsAsync()
    {
        return await _unitOfWork.Departments
            .GetActiveAsync();
    }

    public async Task SetDepartmentAsync(
        Guid assignmentId,
        Guid? departmentId)
    {
        if (assignmentId == Guid.Empty)
            return;

        var assignment =
            await _unitOfWork.TestAssignments
                .GetByIdAsync(assignmentId);

        if (assignment is null)
            return;

        assignment.DepartmentId = departmentId;

        _unitOfWork.TestAssignments.Update(assignment);

        await _unitOfWork.SaveChangesAsync();
    }
    public async Task<List<TestAssignment>> GetPendingForTechnicalManagerAsync()
    {
        return await _unitOfWork.TestAssignments
            .GetPendingForTechnicalManagerAsync();
    }
    public async Task AssignToDepartmentAsync(
    Guid assignmentId,
    Guid? departmentId)
    {
        if (assignmentId == Guid.Empty)
            return;

        if (!departmentId.HasValue || departmentId.Value == Guid.Empty)
            throw new InvalidOperationException(
                "ابتدا باید بخش را انتخاب کنید.");

        var assignment =
            await _unitOfWork.TestAssignments
                .GetByIdAsync(assignmentId);

        if (assignment is null)
            throw new InvalidOperationException(
                "آزمون موردنظر پیدا نشد.");

        assignment.DepartmentId = departmentId;
        

        _unitOfWork.TestAssignments.Update(assignment);

        await _unitOfWork.SaveChangesAsync();
    }
    public async Task<List<TestAssignment>> GetPendingForSectionHeadAsync(
    Guid departmentId)
    {
        return await _unitOfWork.TestAssignments
            .GetPendingForSectionHeadAsync(departmentId);
    }

    public async Task<List<TestAssignment>> GetPendingResultApprovalForSectionHeadAsync(
    Guid departmentId)
    {
        if (departmentId == Guid.Empty)
            return new List<TestAssignment>();

        return await _unitOfWork.TestAssignments
            .GetPendingResultApprovalForSectionHeadAsync(
                departmentId);
    }
    public async Task<List<TestAssignment>> GetPendingResultApprovalForTechnicalManagerAsync()
    {
        return await _unitOfWork.TestAssignments
            .GetPendingResultApprovalForTechnicalManagerAsync();
    }

    public async Task<List<TestAssignment>> GetPendingResultApprovalForDirectorAsync()
    {
        return await _unitOfWork.TestAssignments
            .GetPendingResultApprovalForDirectorAsync();
    }
    public async Task<List<Employee>> GetAnalystsByDepartmentAsync(
    Guid departmentId)
    {
        if (departmentId == Guid.Empty)
            return new List<Employee>();

        var employees =
            await _unitOfWork.Employees
                .GetByDepartmentAsync(departmentId);

        return employees
            .Where(x => x.EmployeeRoles
                .Any(r => r.Role != null &&
                          r.Role.Code == "ANALYST"))
            .ToList();
    }
    public async Task AssignToAnalystAsync(
    Guid assignmentId,
    Guid? analystId)
    {
        if (assignmentId == Guid.Empty)
            return;

        if (!analystId.HasValue || analystId.Value == Guid.Empty)
            throw new InvalidOperationException(
                "ابتدا باید کارشناس را انتخاب کنید.");

        var assignment =
            await _unitOfWork.TestAssignments
                .GetByIdAsync(assignmentId);

        if (assignment is null)
            throw new InvalidOperationException(
                "آزمون موردنظر پیدا نشد.");

        assignment.AnalystId = analystId;

        _unitOfWork.TestAssignments.Update(assignment);

        await _unitOfWork.SaveChangesAsync();
    }
    public async Task AssignToAnalystAsync(
    List<Guid> assignmentIds,
    Guid analystId)
    {
        if (assignmentIds == null || assignmentIds.Count == 0)
            throw new InvalidOperationException(
                "حداقل یک آزمون را انتخاب کنید.");

        if (analystId == Guid.Empty)
            throw new InvalidOperationException(
                "ابتدا باید کارشناس را انتخاب کنید.");

        await _unitOfWork.TestAssignments
            .AssignToAnalystAsync(
                assignmentIds,
                analystId);
    }
    public async Task<List<TestAssignment>> GetPendingForAnalystAsync(
    Guid analystId)
    {
        if (analystId == Guid.Empty)
            return new List<TestAssignment>();

        return await _unitOfWork.TestAssignments
            .GetPendingForAnalystAsync(analystId);
    }

    public bool IsResultOutOfLimit(TestAssignment assignment)
    {
        if (assignment is null)
            return false;

        if (string.IsNullOrWhiteSpace(assignment.Result))
            return false;

        var referenceLimit = assignment.Test?.ReferenceLimits?
            .OrderByDescending(x => x.Priority)
            .FirstOrDefault();

        if (referenceLimit is null)
            return false;

        if (!decimal.TryParse(
            assignment.Result,
            out var resultValue))
        {
            return false;
        }

        if (referenceLimit.MinValue.HasValue &&
            resultValue < referenceLimit.MinValue.Value)
        {
            return true;
        }

        if (referenceLimit.MaxValue.HasValue &&
            resultValue > referenceLimit.MaxValue.Value)
        {
            return true;
        }

        return false;
    }
    public async Task SaveResultAsync(
    Guid assignmentId,
    string? result,
    string? comment)
    {
        if (assignmentId == Guid.Empty)
            return;

        var assignment =
            await _unitOfWork.TestAssignments
                .GetByIdAsync(assignmentId);

        if (assignment is null)
            throw new InvalidOperationException(
                "آزمون موردنظر پیدا نشد.");

        assignment.Result = result;
        assignment.Comment = comment;
        assignment.CompletedAt = DateTime.UtcNow;

        _unitOfWork.TestAssignments.Update(assignment);

        await _unitOfWork.SaveChangesAsync();
    }
    public async Task ApproveResultBySectionAsync(Guid assignmentId)
    {
        if (assignmentId == Guid.Empty)
            throw new InvalidOperationException(
                "آزمون مشخص نشده است.");

        var assignment = await _unitOfWork
            .TestAssignments
            .GetByIdAsync(assignmentId);

        if (assignment is null)
            throw new InvalidOperationException(
                "آزمون موردنظر پیدا نشد.");

        if (string.IsNullOrWhiteSpace(assignment.Result))
            throw new InvalidOperationException(
                "برای این آزمون هنوز نتیجه‌ای ثبت نشده است.");

        if (assignment.IsApprovedBySection)
            throw new InvalidOperationException(
                "این نتیجه قبلاً تأیید شده است.");

        assignment.IsApprovedBySection = true;

        _unitOfWork
            .TestAssignments
            .Update(assignment);

        await _unitOfWork.SaveChangesAsync();
    }
    public async Task ApproveResultByTechManagerAsync(Guid assignmentId)
    {
        if (assignmentId == Guid.Empty)
            throw new InvalidOperationException(
                "آزمون مشخص نشده است.");

        var assignment = await _unitOfWork
            .TestAssignments
            .GetByIdAsync(assignmentId);

        if (assignment is null)
            throw new InvalidOperationException(
                "آزمون موردنظر پیدا نشد.");

        if (string.IsNullOrWhiteSpace(assignment.Result))
            throw new InvalidOperationException(
                "برای این آزمون هنوز نتیجه‌ای ثبت نشده است.");

        if (!assignment.IsApprovedBySection)
            throw new InvalidOperationException(
                "این نتیجه هنوز توسط مسئول بخش تأیید نشده است.");

        if (assignment.IsApprovedByTechManager)
            throw new InvalidOperationException(
                "این نتیجه قبلاً توسط مسئول فنی تأیید شده است.");

        assignment.IsApprovedByTechManager = true;

        _unitOfWork
            .TestAssignments
            .Update(assignment);

        await _unitOfWork.SaveChangesAsync();
    }
    public async Task ApproveResultByDirectorAsync(Guid assignmentId)
    {
        if (assignmentId == Guid.Empty)
            throw new InvalidOperationException(
                "آزمون مشخص نشده است.");

        var assignment = await _unitOfWork
            .TestAssignments
            .GetByIdAsync(assignmentId);

        if (assignment is null)
            throw new InvalidOperationException(
                "آزمون موردنظر پیدا نشد.");

        if (string.IsNullOrWhiteSpace(assignment.Result))
            throw new InvalidOperationException(
                "برای این آزمون هنوز نتیجه‌ای ثبت نشده است.");

        if (!assignment.IsApprovedBySection)
            throw new InvalidOperationException(
                "این نتیجه هنوز توسط مسئول بخش تأیید نشده است.");

        if (!assignment.IsApprovedByTechManager)
            throw new InvalidOperationException(
                "این نتیجه هنوز توسط مسئول فنی تأیید نشده است.");

        if (assignment.IsApprovedByDirector)
            throw new InvalidOperationException(
                "این نتیجه قبلاً توسط مدیر ارشد تأیید شده است.");

        assignment.IsApprovedByDirector = true;

        _unitOfWork
            .TestAssignments
            .Update(assignment);

        await _unitOfWork.SaveChangesAsync();
    }
}