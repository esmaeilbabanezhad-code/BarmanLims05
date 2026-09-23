namespace Barman.Application.Interfaces.Services;

public sealed class DatabaseBackupSettings
{
    public bool Enabled { get; set; } = true;

    public string Mode { get; set; } = "Daily";

    public string DailyTime { get; set; } = "02:00";

    public string WeeklyDay { get; set; } = "Friday";

    public string WeeklyTime { get; set; } = "02:00";

    public int RetentionCount { get; set; } = 30;
}