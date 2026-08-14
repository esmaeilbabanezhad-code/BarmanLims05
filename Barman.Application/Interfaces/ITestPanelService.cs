using Barman.Domain.Entities;

namespace Barman.Application.Interfaces;

public interface ITestPanelService
{
    Task<List<TestPanel>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<TestPanel?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<TestPanel> CreateAsync(
        TestPanel panel,
        CancellationToken cancellationToken = default);

    Task<TestPanel?> UpdateAsync(
        TestPanel panel,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<TestPanel> ActivateAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<TestPanel> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}