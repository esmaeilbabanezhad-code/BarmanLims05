using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
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
            .OrderBy(x => x.Test.Code)
            .ToListAsync();
    }

    public async Task<List<TestAssignment>> GetByReceptionIdAsync(Guid receptionId)
    {
        return await _context.TestAssignments
            .Where(x => x.Sample.ReceptionId == receptionId)
            .Include(x => x.Sample)
            .Include(x => x.Test)
            .OrderBy(x => x.Sample.SampleCode)
            .ThenBy(x => x.Test.Code)
            .ToListAsync();
    }
    public async Task<List<TestAssignment>> GetPendingForTechnicalManagerAsync()
    {
        return await _context.TestAssignments
            .Where(x => !x.IsApprovedByTechManager)
            .Include(x => x.Sample)
            .Include(x => x.Test)
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
                !x.IsApprovedBySection &&
                x.AnalystId == null)
            .Include(x => x.Sample)
            .Include(x => x.Test)
            .Include(x => x.Department)
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
                x.AnalystId != null &&
                !string.IsNullOrWhiteSpace(x.Result) &&
                !x.IsApprovedBySection)
            .Include(x => x.Sample)
            .Include(x => x.Test)
                .ThenInclude(x => x.ReferenceLimits)
            .Include(x => x.Department)
            .OrderBy(x => x.Sample.SampleCode)
            .ThenBy(x => x.Test.Code)
            .ToListAsync();
    }
    public async Task<List<TestAssignment>> GetPendingResultApprovalForTechnicalManagerAsync()
    {
        return await _context.TestAssignments
            .Where(x =>
                   x.IsApprovedBySection &&
                   !string.IsNullOrWhiteSpace(x.Result) &&
                   !x.IsApprovedByTechManager)
            .Include(x => x.Sample)
            .Include(x => x.Test)
                .ThenInclude(x => x.ReferenceLimits)
            .Include(x => x.Department)
            .OrderBy(x => x.Sample.SampleCode)
            .ThenBy(x => x.Test.Code)
            .ToListAsync();
    }
    public async Task<List<TestAssignment>> GetPendingResultApprovalForDirectorAsync()
    {
        return await _context.TestAssignments
            .Where(x =>
                x.IsApprovedBySection &&
                x.IsApprovedByTechManager &&
                !string.IsNullOrWhiteSpace(x.Result) &&
                !x.IsApprovedByDirector)
            .Include(x => x.Sample)
            .Include(x => x.Test)
                .ThenInclude(x => x.ReferenceLimits)
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
                string.IsNullOrWhiteSpace(x.Result))
            .Include(x => x.Sample)
            .Include(x => x.Test)
             .ThenInclude(x => x.ReferenceLimits)
            .Include(x => x.Department)
            .OrderBy(x => x.Sample.SampleCode)
            .ThenBy(x => x.Test.Code)
            .ToListAsync();
    }
    public async Task AssignToAnalystAsync(
    List<Guid> assignmentIds,
    Guid analystId)
    {
        if (assignmentIds == null || assignmentIds.Count == 0)
            return;

        if (analystId == Guid.Empty)
            throw new InvalidOperationException(
                "کارشناس مشخص نشده است.");

        var assignments =
            await _context.TestAssignments
                .Where(x =>
                    assignmentIds.Contains(x.Id) &&
                    x.AnalystId == null)
                .ToListAsync();

        foreach (var assignment in assignments)
        {
            assignment.AnalystId = analystId;
        }

        await _context.SaveChangesAsync();
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