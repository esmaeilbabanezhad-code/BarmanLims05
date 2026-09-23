using Barman.Domain.Entities;

namespace Barman.Application.Interfaces.Repositories;

public interface ITestPanelRepository
{
    Task<List<TestPanel>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<TestPanel?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<TestPanel?> GetByCodeAsync(
    string code,
    CancellationToken cancellationToken = default);

    Task AddAsync(
        TestPanel entity,
        CancellationToken cancellationToken = default);

    void Update(TestPanel entity);

    void Delete(TestPanel entity);
}