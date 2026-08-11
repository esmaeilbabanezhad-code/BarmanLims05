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

    public void Update(TestAssignment assignment)
    {
        _context.TestAssignments.Update(assignment);
    }

    public void Delete(TestAssignment assignment)
    {
        _context.TestAssignments.Remove(assignment);
    }
}