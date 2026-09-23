using Barman.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace Barman.Persistence.Services;

public sealed class PostgreSQLDatabaseBackupService
    : IDatabaseBackupService
{
    private readonly IConfiguration _configuration;
    private readonly DatabaseBackupSettingsService _settingsService;

    private const string PgDumpPath =
        @"C:\Program Files\PostgreSQL\18\bin\pg_dump.exe";

    public PostgreSQLDatabaseBackupService(
        IConfiguration configuration,
        DatabaseBackupSettingsService settingsService)
    {
        _configuration = configuration;
        _settingsService = settingsService;
    }

    public async Task<string> CreateBackupAsync(
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        var connectionString =
            _configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "DefaultConnection is not configured.");

        var builder =
            new Npgsql.NpgsqlConnectionStringBuilder(connectionString);

        if (!File.Exists(PgDumpPath))
        {
            throw new FileNotFoundException(
                "PostgreSQL pg_dump.exe was not found.",
                PgDumpPath);
        }

        var backupDirectory =
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.CommonApplicationData),
                "BarmanLims",
                "Backups");

        Directory.CreateDirectory(backupDirectory);

        var timestamp =
            DateTime.Now.ToString("yyyyMMdd_HHmmss");

        var safeReason =
            string.IsNullOrWhiteSpace(reason)
                ? "DatabaseBackup"
                : string.Concat(
                    reason.Where(
                        c => char.IsLetterOrDigit(c) ||
                             c == '-' ||
                             c == '_'));

        if (string.IsNullOrWhiteSpace(safeReason))
        {
            safeReason = "DatabaseBackup";
        }

        var backupFileName =
            $"BarmanLims_{safeReason}_{timestamp}.backup";

        var backupPath =
            Path.Combine(
                backupDirectory,
                backupFileName);

        var arguments =
            $"--host \"{builder.Host}\" " +
            $"--port \"{builder.Port}\" " +
            $"--username \"{builder.Username}\" " +
            $"--dbname \"{builder.Database}\" " +
            $"--format custom " +
            $"--file \"{backupPath}\"";

        var startInfo = new ProcessStartInfo
        {
            FileName = PgDumpPath,
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        if (!string.IsNullOrWhiteSpace(builder.Password))
        {
            startInfo.Environment["PGPASSWORD"] =
                builder.Password;
        }

        using var process = new Process
        {
            StartInfo = startInfo
        };

        process.Start();

        var standardOutputTask =
            process.StandardOutput.ReadToEndAsync(
                cancellationToken);

        var standardErrorTask =
            process.StandardError.ReadToEndAsync(
                cancellationToken);

        await process.WaitForExitAsync(cancellationToken);

        var standardOutput =
            await standardOutputTask;

        var standardError =
            await standardErrorTask;

        if (process.ExitCode != 0)
        {
            if (File.Exists(backupPath))
            {
                File.Delete(backupPath);
            }

            throw new InvalidOperationException(
                $"PostgreSQL backup failed. " +
                $"ExitCode: {process.ExitCode}. " +
                $"Error: {standardError}");
        }

        if (!File.Exists(backupPath))
        {
            throw new InvalidOperationException(
                "PostgreSQL backup command completed " +
                "but the backup file was not created.");
        }

        var fileInfo =
            new FileInfo(backupPath);

        if (fileInfo.Length == 0)
        {
            fileInfo.Delete();

            throw new InvalidOperationException(
                "PostgreSQL backup file is empty.");
        }

        CleanupOldBackups(
            backupDirectory,
            cancellationToken);

        return backupPath;
    }

    private void CleanupOldBackups(
        string backupDirectory,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var retentionCount =
            _settingsService
                .GetSettings()
                .RetentionCount;

        if (retentionCount < 1)
        {
            retentionCount = 1;
        }

        var backupFiles =
            new DirectoryInfo(backupDirectory)
                .GetFiles("BarmanLims_*.backup")
                .OrderByDescending(
                    file => file.CreationTime)
                .ToList();

        if (backupFiles.Count <= retentionCount)
        {
            return;
        }

        foreach (var file in backupFiles.Skip(retentionCount))
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                file.Delete();
            }
            catch
            {
                // Do not fail a successful backup
                // because an old backup could not be deleted.
            }
        }
    }
}