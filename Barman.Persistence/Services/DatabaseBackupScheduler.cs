using Barman.Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Barman.Persistence.Services;

public sealed class DatabaseBackupScheduler : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly DatabaseBackupSettingsService _settingsService;
    private readonly ILogger<DatabaseBackupScheduler> _logger;

    private string? _lastRunKey;

    public DatabaseBackupScheduler(
        IServiceScopeFactory scopeFactory,
        DatabaseBackupSettingsService settingsService,
        ILogger<DatabaseBackupScheduler> logger)
    {
        _scopeFactory = scopeFactory;
        _settingsService = settingsService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var settings =
                    _settingsService.GetSettings();

                if (!settings.Enabled)
                {
                    _logger.LogInformation(
                        "Automatic database backup is disabled.");

                    await Task.Delay(
                        TimeSpan.FromMinutes(1),
                        stoppingToken);

                    continue;
                }

                var now = DateTime.Now;

                if (IsBackupDue(settings, now))
                {
                    var runKey =
                        $"{now:yyyyMMdd_HHmm}_{settings.Mode}_{settings.DailyTime}_{settings.WeeklyDay}_{settings.WeeklyTime}";

                    if (_lastRunKey != runKey)
                    {
                        _lastRunKey = runKey;

                        using var scope =
                            _scopeFactory.CreateScope();

                        var backupService =
                            scope.ServiceProvider
                                .GetRequiredService<IDatabaseBackupService>();

                        var backupPath =
                            await backupService.CreateBackupAsync(
                                "Scheduled",
                                stoppingToken);

                        _logger.LogInformation(
                            "Automatic database backup completed successfully: {BackupPath}",
                            backupPath);
                    }
                }

                await Task.Delay(
                    TimeSpan.FromSeconds(20),
                    stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Automatic database backup failed.");

                await Task.Delay(
                    TimeSpan.FromSeconds(20),
                    stoppingToken);
            }
        }
    }

    private static bool IsBackupDue(
        DatabaseBackupSettings settings,
        DateTime now)
    {
        var timeText =
            settings.Mode.Equals(
                "Weekly",
                StringComparison.OrdinalIgnoreCase)
                ? settings.WeeklyTime
                : settings.DailyTime;

        if (!TimeSpan.TryParse(
                timeText,
                CultureInfo.InvariantCulture,
                out var backupTime))
        {
            backupTime =
                new TimeSpan(2, 0, 0);
        }

        if (now.Hour != backupTime.Hours ||
            now.Minute != backupTime.Minutes)
        {
            return false;
        }

        if (settings.Mode.Equals(
                "Weekly",
                StringComparison.OrdinalIgnoreCase))
        {
            var targetDay =
                ParseDay(settings.WeeklyDay);

            return now.DayOfWeek == targetDay;
        }

        return true;
    }

    private static DayOfWeek ParseDay(
        string? day)
    {
        return day?.ToLowerInvariant() switch
        {
            "saturday" => DayOfWeek.Saturday,
            "sunday" => DayOfWeek.Sunday,
            "monday" => DayOfWeek.Monday,
            "tuesday" => DayOfWeek.Tuesday,
            "wednesday" => DayOfWeek.Wednesday,
            "thursday" => DayOfWeek.Thursday,
            "friday" => DayOfWeek.Friday,
            _ => DayOfWeek.Friday
        };
    }
}