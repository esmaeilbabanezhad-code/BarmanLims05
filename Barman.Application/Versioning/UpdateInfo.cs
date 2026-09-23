namespace Barman.Application.Versioning;

public sealed class UpdateInfo
{
    public bool IsUpdateAvailable { get; init; }

    public string CurrentVersion { get; init; } = string.Empty;

    public string LatestVersion { get; init; } = string.Empty;

    public string? ReleaseDate { get; init; }

    public string? ReleaseNotes { get; init; }

    public string? DownloadUrl { get; init; }
}
