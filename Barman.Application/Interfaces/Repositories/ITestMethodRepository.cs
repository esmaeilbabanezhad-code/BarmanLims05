using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface ITestMethodRepository
{
    Task AddAsync(TestMethod testMethod);

    Task<TestMethod?> GetByIdAsync(Guid id);

    Task<TestMethod?> GetByCodeAsync(string code);

    Task<List<TestMethod>> GetAllAsync();

    void Update(TestMethod testMethod);

    void Delete(TestMethod testMethod);
}
