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
}