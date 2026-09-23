namespace Barman.Application.Interfaces.Services;

public interface IDatabaseBackupService
{
    Task<string> CreateBackupAsync(
        string? reason = null,
        CancellationToken cancellationToken = default);
}