namespace Barman.Application.Interfaces.Services;

public interface IApplicationDatabaseUpgradeService
{
    Task<string?> GetCurrentMigrationAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetPendingMigrationsAsync(
        CancellationToken cancellationToken = default);

    Task ApplyMigrationsAsync(
        CancellationToken cancellationToken = default);
}