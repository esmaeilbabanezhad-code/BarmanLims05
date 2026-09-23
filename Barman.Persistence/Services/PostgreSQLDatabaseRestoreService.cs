using Barman.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Barman.Persistence.Services;

public sealed class PostgreSQLDatabaseRestoreService
    : IDatabaseRestoreService
{
    private readonly IDatabaseRestoreRequestService _restoreRequestService;
    private readonly IConfiguration _configuration;
    private readonly IDatabaseBackupService _backupService;

    private const string PgRestorePath =
        @"C:\Program Files\PostgreSQL\18\bin\pg_restore.exe";

    public PostgreSQLDatabaseRestoreService(
    IConfiguration configuration,
    IDatabaseBackupService backupService,
    IDatabaseRestoreRequestService restoreRequestService)
    {
        _configuration = configuration;
        _backupService = backupService;
        _restoreRequestService = restoreRequestService;
    }

    public async Task<DatabaseBackupValidationResult> ValidateBackupAsync(
        string backupPath,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(backupPath))
        {
            return new(
                false,
                "فایل Backup مشخص نشده است.",
                null,
                null,
                0);
        }

        if (!File.Exists(backupPath))
        {
            return new(
                false,
                "فایل Backup پیدا نشد.",
                null,
                null,
                0);
        }

        if (!File.Exists(PgRestorePath))
        {
            throw new FileNotFoundException(
                "PostgreSQL pg_restore.exe was not found.",
                PgRestorePath);
        }

        var connectionString =
            _configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "DefaultConnection is not configured.");

        var builder =
            new Npgsql.NpgsqlConnectionStringBuilder(
                connectionString);

        var startInfo = new ProcessStartInfo
        {
            FileName = PgRestorePath,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        startInfo.ArgumentList.Add("--list");
        startInfo.ArgumentList.Add(backupPath);

        using var process = new Process
        {
            StartInfo = startInfo
        };

        process.Start();

        var outputTask =
            process.StandardOutput.ReadToEndAsync(
                cancellationToken);

        var errorTask =
            process.StandardError.ReadToEndAsync(
                cancellationToken);

        await process.WaitForExitAsync(
            cancellationToken);

        var output =
            await outputTask;

        var error =
            await errorTask;

        if (process.ExitCode != 0)
        {
            return new(
                false,
                $"فایل Backup معتبر نیست یا قابل خواندن نیست. {error}",
                null,
                null,
                0);
        }

        var lines =
            output.Split(
                Environment.NewLine,
                StringSplitOptions.RemoveEmptyEntries);

        var tocCount =
            lines.Count(
                x => Regex.IsMatch(
                    x,
                    @"^\d+;"));

        var databaseName =
            builder.Database;

        return new(
            true,
            "Backup معتبر است و ساختار فایل قابل خواندن است.",
            databaseName,
            "PostgreSQL Custom Format",
            tocCount);
    }

    public async Task<RestorePreparationResult> PrepareRestoreAsync(
        string backupPath,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(backupPath))
        {
            throw new ArgumentException(
                "Backup path is required.",
                nameof(backupPath));
        }

        if (!File.Exists(backupPath))
        {
            throw new FileNotFoundException(
                "Backup file was not found.",
                backupPath);
        }

        var validation =
            await ValidateBackupAsync(
                backupPath,
                cancellationToken);

        if (!validation.IsValid)
        {
            throw new InvalidOperationException(
                validation.Message);
        }

        // ایجاد Backup اضطراری از Database فعلی
        var emergencyBackupPath =
            await _backupService.CreateBackupAsync(
                "BeforeRestore",
                cancellationToken);

        var restoreDirectory =
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.CommonApplicationData),
                "BarmanLims",
                "Restore");

        Directory.CreateDirectory(
            restoreDirectory);

        var restoreBackupPath =
            Path.Combine(
                restoreDirectory,
                $"RestoreSource_{DateTime.Now:yyyyMMdd_HHmmss}.backup");

        File.Copy(
            backupPath,
            restoreBackupPath,
            overwrite: false);

        var connectionString =
    _configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "DefaultConnection is not configured.");

        var connectionBuilder =
            new Npgsql.NpgsqlConnectionStringBuilder(
                connectionString);

        var request =
            new DatabaseRestoreRequest
            {
                BackupPath = restoreBackupPath,
                EmergencyBackupPath = emergencyBackupPath,
                TargetDatabase = connectionBuilder.Database,
                CreatedAt = DateTime.Now,
                Status = "Pending"
            };

        var requestPath =
            await _restoreRequestService.CreateRequestAsync(
                request,
                cancellationToken);

        return new RestorePreparationResult(
                restoreBackupPath,
                emergencyBackupPath,
                requestPath);
      }
    public async Task<RestoreExecutionResult> ExecuteRestoreAsync(
    string restoreRequestPath,
    CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(restoreRequestPath))
        {
            throw new ArgumentException(
                "Restore request path is required.",
                nameof(restoreRequestPath));
        }

        if (!File.Exists(restoreRequestPath))
        {
            throw new FileNotFoundException(
                "Restore request file was not found.",
                restoreRequestPath);
        }

        var workerDllPath =
            Path.Combine(
                AppContext.BaseDirectory,
                "..",
                "..",
                "..",
                "..",
                "Barman.DatabaseRestoreWorker",
                "bin",
                "Debug",
                "net9.0",
                "Barman.DatabaseRestoreWorker.dll");

        workerDllPath =
            Path.GetFullPath(workerDllPath);

        if (!File.Exists(workerDllPath))
        {
            throw new FileNotFoundException(
                "Database Restore Worker was not found.",
                workerDllPath);
        }

        var startInfo =
            new ProcessStartInfo
            {
                FileName = "dotnet",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

        startInfo.ArgumentList.Add(workerDllPath);
        startInfo.ArgumentList.Add(restoreRequestPath);

        using var process =
            new Process
            {
                StartInfo = startInfo
            };

        process.Start();

        var outputTask =
            process.StandardOutput.ReadToEndAsync(
                cancellationToken);

        var errorTask =
            process.StandardError.ReadToEndAsync(
                cancellationToken);

        await process.WaitForExitAsync(
            cancellationToken);

        var output =
            await outputTask;

        var error =
            await errorTask;

        if (process.ExitCode == 0)
        {
            return new RestoreExecutionResult(
                true,
                "Restore با موفقیت انجام شد.",
                process.ExitCode,
                null);
        }

        var errorMessage =
            string.IsNullOrWhiteSpace(error)
                ? output
                : error;

        return new RestoreExecutionResult(
            false,
            "اجرای Restore ناموفق بود.",
            process.ExitCode,
            errorMessage);
    }
}