using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class TestRepository : ITestRepository
{
    private readonly ApplicationDbContext _context;

    public TestRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Test test)
    {
        await _context.Tests.AddAsync(test);
    }

    public async Task<Test?> GetByIdAsync(Guid id)
    {
        return await _context.Tests
            .Include(x => x.TestMethod)
            .Include(x => x.Instrument)
            .Include(x => x.ReferenceLimits)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Test?> GetByCodeAsync(string code)
    {
        return await _context.Tests
            .FirstOrDefaultAsync(x => x.Code == code);
    }

    public async Task<List<Test>> GetAllAsync()
    {
        return await _context.Tests
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Code)
            .ToListAsync();
    }

    public async Task<List<Test>> GetDeletedAsync()
    {
        return await _context.Tests
            .Where(x => x.IsDeleted)
            .OrderBy(x => x.Code)
            .ToListAsync();
    }
    public void Update(Test test)
    {
        _context.Tests.Update(test);
    }

    public void Delete(Test test)
    {
        _context.Tests.Remove(test);
    }
}