using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface ITestResultDefinitionRepository
{
    Task<List<TestResultDefinition>> GetByTestIdAsync(
        Guid testId,
        CancellationToken cancellationToken = default);

    Task<TestResultDefinition?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        TestResultDefinition entity,
        CancellationToken cancellationToken = default);

    void Update(
        TestResultDefinition entity);

    void Delete(
        TestResultDefinition entity);
}