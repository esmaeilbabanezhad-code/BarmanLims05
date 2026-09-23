using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Domain.Enums;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class TestLimitChangeRequestRepository
    : ITestLimitChangeRequestRepository
{
    private readonly ApplicationDbContext _context;

    public TestLimitChangeRequestRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        TestLimitChangeRequest request)
    {
        await _context.TestLimitChangeRequests
            .AddAsync(request);
    }

    public async Task<TestLimitChangeRequest?> GetByIdAsync(
    Guid id)
    {
        if (id == Guid.Empty)
            return null;

        return await _context.TestLimitChangeRequests
            .Include(x => x.Test)
            .Include(x => x.ReferenceLimit)
            .Include(x => x.TestResultSetItem)
                .ThenInclude(x => x!.TestResultDefinition)
            .Include(x => x.RequestedByEmployee)
            .Include(x => x.ApprovedByEmployee)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<TestLimitChangeRequest>>
    GetPendingForSectionHeadAsync(Guid departmentId)
    {
        if (departmentId == Guid.Empty)
            return new List<TestLimitChangeRequest>();

        return await _context.TestLimitChangeRequests
            .Include(x => x.Test)
            .Include(x => x.ReferenceLimit)
            .Include(x => x.TestResultSetItem)
                .ThenInclude(x => x!.TestResultDefinition)
            .Include(x => x.RequestedByEmployee)
            .Include(x => x.ApprovedByEmployee)
            .Where(x =>
                x.Status == TestLimitChangeRequestStatus.Pending &&
                x.Test.DepartmentId == departmentId &&
                !x.IsDeleted)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    }

    public void Update(
        TestLimitChangeRequest request)
    {
        _context.TestLimitChangeRequests.Update(request);
    }
}