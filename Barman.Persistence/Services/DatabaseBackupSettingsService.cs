using Barman.Application.Interfaces.Services;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace Barman.Persistence.Services;

public sealed class DatabaseBackupSettingsService
{
    private readonly string _settingsFilePath;

    private DatabaseBackupSettings _settings;

    public DatabaseBackupSettingsService(
        IHostEnvironment environment)
    {
        var directory = Path.Combine(
            environment.ContentRootPath,
            "App_Data");

        Directory.CreateDirectory(directory);

        _settingsFilePath =
            Path.Combine(
                directory,
                "database-backup-settings.json");

        _settings = Load();
    }

    public DatabaseBackupSettings GetSettings()
    {
        return _settings;
    }

    public async Task SaveAsync(
        DatabaseBackupSettings settings,
        CancellationToken cancellationToken = default)
    {
        _settings = settings;

        var json = JsonSerializer.Serialize(
            settings,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        await File.WriteAllTextAsync(
            _settingsFilePath,
            json,
            cancellationToken);
    }

    private DatabaseBackupSettings Load()
    {
        try
        {
            if (!File.Exists(_settingsFilePath))
                return new DatabaseBackupSettings();

            var json =
                File.ReadAllText(
                    _settingsFilePath);

            return JsonSerializer.Deserialize<DatabaseBackupSettings>(
                       json)
                   ?? new DatabaseBackupSettings();
        }
        catch
        {
            return new DatabaseBackupSettings();
        }
    }
}