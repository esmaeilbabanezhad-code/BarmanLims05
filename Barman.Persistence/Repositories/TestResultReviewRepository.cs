using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class TestResultReviewRepository : ITestResultReviewRepository
{
    private readonly ApplicationDbContext _context;

    public TestResultReviewRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(TestResultReview review)
    {
        await _context.TestResultReviews.AddAsync(review);
    }

    public async Task<TestResultReview?> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            return null;

        return await _context.TestResultReviews
            .Include(x => x.TestAssignment)
            .Include(x => x.ReviewerEmployee)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<TestResultReview>> GetByAssignmentIdAsync(
        Guid testAssignmentId)
    {
        if (testAssignmentId == Guid.Empty)
            return new List<TestResultReview>();

        return await _context.TestResultReviews
            .Where(x => x.TestAssignmentId == testAssignmentId)
            .Include(x => x.ReviewerEmployee)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<TestResultReview>> GetByReviewerEmployeeIdAsync(
        Guid reviewerEmployeeId)
    {
        if (reviewerEmployeeId == Guid.Empty)
            return new List<TestResultReview>();

        return await _context.TestResultReviews
            .Where(x => x.ReviewerEmployeeId == reviewerEmployeeId)
            .Include(x => x.TestAssignment)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public void Update(TestResultReview review)
    {
        _context.TestResultReviews.Update(review);
    }

    public void Delete(TestResultReview review)
    {
        _context.TestResultReviews.Remove(review);
    }
}
