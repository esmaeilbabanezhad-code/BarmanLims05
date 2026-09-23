using Barman.Application.DTOs.Reception;
using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Domain.Enums;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class TestAssignmentRepository : ITestAssignmentRepository
{
    private readonly ApplicationDbContext _context;

    public TestAssignmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(TestAssignment assignment)
    {
        await _context.TestAssignments.AddAsync(assignment);
    }

    public async Task<TestAssignment?> GetByIdAsync(Guid id)
    {
        return await _context.TestAssignments
            .Include(x => x.Sample)
            .Include(x => x.Test)
                .ThenInclude(x => x.ReferenceLimits)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
    public async Task<List<TestAssignment>> GetBySampleIdAsync(Guid sampleId)
    {
        return await _context.TestAssignments
            .Where(x => x.SampleId == sampleId)
            .Include(x => x.Test)
                .ThenInclude(x => x.ReferenceLimits)
            .Include(x => x.Test)
                .ThenInclude(x => x.TestMethod)
            .Include(x => x.Test)
                .ThenInclude(x => x.Instrument)
            .Include(x => x.Department)
            .OrderBy(x => x.Test.Code)
            .ToListAsync();
    }
    public async Task<List<TestAssignment>> GetBySampleIdAndTestPanelIdAsync(
    Guid sampleId,
    Guid testPanelId)
    {
        if (sampleId == Guid.Empty ||
            testPanelId == Guid.Empty)
            return new List<TestAssignment>();

        return await _context.TestAssignments
            .Where(x =>
                x.SampleId == sampleId &&
                x.TestPanelId == testPanelId)
            .Include(x => x.Sample)
            .Include(x => x.Test)
            .Include(x => x.TestPanel)
            .Include(x => x.Department)
            .OrderBy(x => x.Test.Code)
            .ToListAsync();
    }
    public async Task<List<TestAssignment>> GetByReceptionIdAsync(Guid receptionId)
    {
        return await _context.TestAssignments
            .Where(x => x.Sample.ReceptionId == receptionId)
            .Include(x => x.Sample)
            .Include(x => x.Test)
            .Include(x => x.Department)
            .OrderBy(x => x.Sample.SampleCode)
            .ThenBy(x => x.Test.Code)
            .ToListAsync();
    }

    public async Task<List<TestAssignment>> GetForFinalReportByReceptionIdAsync(
    Guid receptionId)
    {
        if (receptionId == Guid.Empty)
            return new List<TestAssignment>();

        return await _context.TestAssignments
            .Where(x => x.Sample.ReceptionId == receptionId)

            .Include(x => x.Sample)
                .ThenInclude(x => x.Reception)
                    .ThenInclude(x => x.Customer)

            .Include(x => x.Sample)
                .ThenInclude(x => x.SampleCategory)

            .Include(x => x.Sample)
                .ThenInclude(x => x.Matrix)

            .Include(x => x.Sample)
                .ThenInclude(x => x.StandardSample)

            .Include(x => x.Sample)
                .ThenInclude(x => x.CustomFieldValues)
                    .ThenInclude(x => x.CustomFieldDefinition)

            .Include(x => x.Test)

            .Include(x => x.Test)
                .ThenInclude(x => x.TestMethod)

            .Include(x => x.Test)
                .ThenInclude(x => x.Instrument)

            .Include(x => x.Test)
                .ThenInclude(x => x.ReferenceLimits)

            .Include(x => x.TestResultSet)
                .ThenInclude(x => x.Items)
                    .ThenInclude(x => x.TestResultDefinition)

            .Include(x => x.ResultValues)
                .ThenInclude(x => x.TestResultSetItem)
                    .ThenInclude(x => x.TestResultDefinition)

            .Include(x => x.SelectedLimitRule)

            .Include(x => x.Department)

            .OrderBy(x => x.Sample.SampleCode)
            .ThenBy(x => x.Test.Code)

            .ToListAsync();
    }

    public async Task<List<TestAssignment>> GetForFinalReportBySampleIdAsync(
    Guid sampleId)
    {
        if (sampleId == Guid.Empty)
            return new List<TestAssignment>();

        return await _context.TestAssignments
            .Where(x => x.SampleId == sampleId)

            .Include(x => x.Sample)
                .ThenInclude(x => x.Reception)
                    .ThenInclude(x => x.Customer)

            .Include(x => x.Sample)
                .ThenInclude(x => x.SampleCategory)

            .Include(x => x.Sample)
                .ThenInclude(x => x.Matrix)

            .Include(x => x.Sample)
                .ThenInclude(x => x.StandardSample)

            .Include(x => x.Sample)
                .ThenInclude(x => x.CustomFieldValues)
                    .ThenInclude(x => x.CustomFieldDefinition)

            .Include(x => x.Test)

            .Include(x => x.Test)
                .ThenInclude(x => x.TestMethod)

            .Include(x => x.Test)
                .ThenInclude(x => x.Instrument)

            .Include(x => x.Test)
                .ThenInclude(x => x.ReferenceLimits)

            .Include(x => x.TestResultSet)
                .ThenInclude(x => x.Items)
                    .ThenInclude(x => x.TestResultDefinition)

            .Include(x => x.ResultValues)
                .ThenInclude(x => x.TestResultSetItem)
                    .ThenInclude(x => x.TestResultDefinition)

            .Include(x => x.SelectedLimitRule)

            .Include(x => x.Department)

            .Include(x => x.TechnicalManager)

            .OrderBy(x => x.TestPanelId)
            .ThenBy(x => x.Test.Code)

            .ToListAsync();
    }

    public async Task<List<ReceptionTestStatusDto>> GetReceptionTestStatusesAsync(
    Guid receptionId)
    {
        return await _context.TestAssignments
            .Where(x => x.Sample.ReceptionId == receptionId)
            .Select(x => new ReceptionTestStatusDto
            {
                AssignmentId = x.Id,
                SampleId = x.SampleId,
                TestCode = x.Test.Code,
                TestName = x.Test.Name,
                DepartmentName = x.Department != null
                    ? x.Department.Name
                    : null,
                Unit = x.Unit ?? x.Test.Unit,
                IsQuantitative = x.Test.IsQuantitative,
                HasResult = !string.IsNullOrWhiteSpace(x.Result),
                WorkflowStage = (int)x.WorkflowStage
            })
            .OrderBy(x => x.SampleId)
            .ThenBy(x => x.TestCode)
            .ToListAsync();
    }

    public async Task<List<TestAssignment>> GetPendingForTechnicalManagerAsync(
    Guid technicalManagerId)
    {
        if (technicalManagerId == Guid.Empty)
            return new List<TestAssignment>();

        return await _context.TestAssignments
            .Where(x =>
                x.TechnicalManagerId == technicalManagerId &&
                x.WorkflowStage ==
                    TestAssignmentWorkflowStage.TechnicalManagerAssignment)
            .Include(x => x.Sample)
            .Include(x => x.Test)
            .Include(x => x.TestPanel)
            .Include(x => x.Department)
            .OrderBy(x => x.Sample.SampleCode)
            .ThenBy(x => x.Test.Code)
            .ToListAsync();
    }
    public async Task<List<TestAssignment>> GetPendingForSectionHeadAsync(
    Guid departmentId)
    {
        if (departmentId == Guid.Empty)
            return new List<TestAssignment>();

        return await _context.TestAssignments
            .Where(x =>
                x.DepartmentId == departmentId &&
                x.WorkflowStage ==
                    TestAssignmentWorkflowStage.SectionAssignment)
            .Include(x => x.Sample)
            .Include(x => x.Test)
            .Include(x => x.TestPanel)
            .Include(x => x.Department)
            .Include(x => x.TechnicalManager)
            .OrderBy(x => x.Sample.SampleCode)
            .ThenBy(x => x.Test.Code)
            .ToListAsync();
    }
    public async Task<List<TestAssignment>> GetPendingResultApprovalForSectionHeadAsync(
    Guid departmentId)
    {
        if (departmentId == Guid.Empty)
            return new List<TestAssignment>();

        return await _context.TestAssignments
            .Where(x =>
                x.DepartmentId == departmentId &&
                x.WorkflowStage ==
                    TestAssignmentWorkflowStage.SectionResultApproval)

            .Include(x => x.Sample)

            .Include(x => x.TestPanel)

            .Include(x => x.Test)
                .ThenInclude(x => x.ReferenceLimits)

            .Include(x => x.TestResultSet)
                .ThenInclude(x => x.Items)
                    .ThenInclude(x => x.TestResultDefinition)

            .Include(x => x.Department)

            .OrderBy(x => x.Sample.SampleCode)
            .ThenBy(x => x.Test.Code)

            .ToListAsync();
    }
    public async Task<List<TestAssignment>>
    GetPendingResultApprovalForTechnicalManagerAsync()
    {
        return await _context.TestAssignments
            .Where(x =>
                x.WorkflowStage ==
                    TestAssignmentWorkflowStage.TechnicalManagerResultApproval)
            .Include(x => x.Sample)
            .Include(x => x.Test)
                .ThenInclude(x => x.ReferenceLimits)
            .Include(x => x.TestResultSet)
                .ThenInclude(x => x.Items)
                    .ThenInclude(x => x.TestResultDefinition)
            .Include(x => x.Department)
            .OrderBy(x => x.Sample.SampleCode)
            .ThenBy(x => x.Test.Code)
            .ToListAsync();
    }
    public async Task<List<TestAssignment>> GetPendingResultApprovalForDirectorAsync()
    {
        return await _context.TestAssignments
            .Where(x =>
                x.WorkflowStage ==
                    TestAssignmentWorkflowStage.DirectorResultApproval)

            .Include(x => x.Sample)
                  .ThenInclude(x => x.Reception)
                  .ThenInclude(x => x.Customer)

            .Include(x => x.Sample)
                  .ThenInclude(x => x.SampleCategory)

            .Include(x => x.Sample)
                  .ThenInclude(x => x.Matrix)

            .Include(x => x.Test)
                  .ThenInclude(x => x.ReferenceLimits)

            .Include(x => x.TestResultSet)
                  .ThenInclude(x => x.Items)
                      .ThenInclude(x => x.TestResultDefinition)

            .Include(x => x.Test)
                  .ThenInclude(x => x.TestMethod)

            .Include(x => x.Test)
                  .ThenInclude(x => x.Instrument)

            .Include(x => x.Department)
            .OrderBy(x => x.Sample.SampleCode)
            .ThenBy(x => x.Test.Code)
            .ToListAsync();
    }
    public async Task<List<TestAssignment>> GetPendingForAnalystAsync(
    Guid analystId)
    {
        if (analystId == Guid.Empty)
            return new List<TestAssignment>();

        return await _context.TestAssignments
            .Where(x =>
                  x.AnalystId == analystId &&
                  x.WorkflowStage ==
                 TestAssignmentWorkflowStage.AnalystWork)
            .Include(x => x.Sample)
            .Include(x => x.Test)
                .ThenInclude(x => x.ReferenceLimits)
            .Include(x => x.TestResultSet)
                    .ThenInclude(x => x.Items)
                        .ThenInclude(x => x.TestResultDefinition)
            .Include(x => x.SelectedLimitRule)
            .Include(x => x.Department)
            .Include(x => x.TestPanel)
            .OrderBy(x => x.Sample.SampleCode)
            .ThenBy(x => x.Test.Code)
            .ToListAsync();
    }

    public async Task<List<TestAssignment>> GetPendingForAnalystBySampleAsync(
     Guid analystId,
     Guid sampleId)
    {
        if (analystId == Guid.Empty || sampleId == Guid.Empty)
            return new List<TestAssignment>();

        return await _context.TestAssignments
            .Where(x =>
                x.AnalystId == analystId &&
                x.SampleId == sampleId &&
                x.WorkflowStage ==
                    TestAssignmentWorkflowStage.AnalystWork)
            .Include(x => x.Sample)
            .Include(x => x.Test)
                .ThenInclude(x => x.ReferenceLimits)
            .Include(x => x.TestResultSet)
                .ThenInclude(x => x.Items)
                    .ThenInclude(x => x.TestResultDefinition)
            .Include(x => x.SelectedLimitRule)
            .Include(x => x.Department)
            .OrderBy(x => x.Test.Code)
            .ToListAsync();
    }
    
    public void Update(TestAssignment assignment)
    {
        _context.TestAssignments.Update(assignment);
    }

    public void Delete(TestAssignment assignment)
    {
        _context.TestAssignments.Remove(assignment);
    }
}