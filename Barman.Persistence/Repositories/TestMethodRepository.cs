using Barman.Application.Interfaces.Repositories;
using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Repositories;

public class TestMethodRepository : ITestMethodRepository
{
    private readonly ApplicationDbContext _context;

    public TestMethodRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(TestMethod testMethod)
    {
        await _context.TestMethods.AddAsync(testMethod);
    }

    public async Task<TestMethod?> GetByIdAsync(Guid id)
    {
        return await _context.TestMethods
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<TestMethod?> GetByCodeAsync(string code)
    {
        return await _context.TestMethods
            .FirstOrDefaultAsync(x => x.Code == code);
    }

    public async Task<List<TestMethod>> GetAllAsync()
    {
        return await _context.TestMethods
            .AsNoTracking()
            .ToListAsync();
    }

    public void Update(TestMethod testMethod)
    {
        _context.TestMethods.Update(testMethod);
    }

    public void Delete(TestMethod testMethod)
    {
        _context.TestMethods.Remove(testMethod);
    }
}
