namespace Barman.Application.Interfaces.Services;

public sealed class DatabaseRestoreRequest
{
    public string BackupPath { get; set; } = string.Empty;

    public string EmergencyBackupPath { get; set; } = string.Empty;

    public string TargetDatabase { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public string Status { get; set; } = "Pending";

    public string? ErrorMessage { get; set; }

    public int? ExitCode { get; set; }
}