namespace Barman.Application.Interfaces.Services;

public interface IDatabaseRestoreService
{
    Task<DatabaseBackupValidationResult> ValidateBackupAsync(
        string backupPath,
        CancellationToken cancellationToken = default);

    Task<RestorePreparationResult> PrepareRestoreAsync(
        string backupPath,
        CancellationToken cancellationToken = default);

    Task<RestoreExecutionResult> ExecuteRestoreAsync(
        string restoreRequestPath,
        CancellationToken cancellationToken = default);
}

public sealed record DatabaseBackupValidationResult(
    bool IsValid,
    string Message,
    string? DatabaseName,
    string? DatabaseVersion,
    int TableOfContentsCount);

public sealed record RestorePreparationResult(
    string RestoreSourcePath,
    string EmergencyBackupPath,
    string RestoreRequestPath);

public sealed record RestoreExecutionResult(
    bool IsSuccess,
    string Message,
    int ExitCode,
    string? ErrorMessage);