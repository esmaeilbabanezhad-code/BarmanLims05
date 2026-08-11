using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface ITestRepository
{
    Task AddAsync(Test test);

    Task<Test?> GetByIdAsync(Guid id);

    Task<Test?> GetByCodeAsync(string code);

    Task<List<Test>> GetAllAsync();

    Task<List<Test>> GetDeletedAsync();

    void Update(Test test);

    void Delete(Test test);
}