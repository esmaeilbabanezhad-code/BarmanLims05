using Barman.Application.Interfaces.Services;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Services;

public sealed class ApplicationDatabaseUpgradeService
    : IApplicationDatabaseUpgradeService
{
    private readonly ApplicationDbContext _context;

    public ApplicationDatabaseUpgradeService(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string?> GetCurrentMigrationAsync(
        CancellationToken cancellationToken = default)
    {
        var appliedMigrations =
            await _context.Database
                .GetAppliedMigrationsAsync(cancellationToken);

        return appliedMigrations.LastOrDefault();
    }

    public async Task<IReadOnlyList<string>> GetPendingMigrationsAsync(
        CancellationToken cancellationToken = default)
    {
        return (
            await _context.Database
                .GetPendingMigrationsAsync(cancellationToken)
        ).ToList();
    }

    public async Task ApplyMigrationsAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.Database
            .MigrateAsync(cancellationToken);
    }
}