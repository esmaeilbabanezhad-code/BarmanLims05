using Barman.Application.Versioning;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Barman.Infrastructure.Services.Versioning;

public sealed class UpdateService : IUpdateService
{
    private const string ReleasesUrl =
        "https://api.github.com/repos/esmaeilbabanezhad-code/BarmanLims05/releases/latest";

    private readonly HttpClient _httpClient;

    public UpdateService(HttpClient httpClient)
    {
        _httpClient = httpClient;

        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "BarmanLims-Updater");
    }

    public async Task<UpdateInfo> CheckForUpdateAsync(
        CancellationToken cancellationToken = default)
    {
        var currentVersion = BarmanVersion.Current;

        try
        {
            var release =
                await _httpClient.GetFromJsonAsync<GitHubRelease>(
                    ReleasesUrl,
                    cancellationToken);

            if (release is null ||
                string.IsNullOrWhiteSpace(release.TagName))
            {
                return new UpdateInfo
                {
                    CurrentVersion = currentVersion,
                    LatestVersion = currentVersion,
                    IsUpdateAvailable = false
                };
            }

            var latestVersion =
                NormalizeVersion(release.TagName);

            var isUpdateAvailable =
                CompareVersions(
                    latestVersion,
                    currentVersion) > 0;

            return new UpdateInfo
            {
                CurrentVersion = currentVersion,
                LatestVersion = latestVersion,
                IsUpdateAvailable = isUpdateAvailable,
                ReleaseDate = release.PublishedAt,
                ReleaseNotes = release.Body,
                DownloadUrl = GetDownloadUrl(release)
            };
        }
        catch
        {
            return new UpdateInfo
            {
                CurrentVersion = currentVersion,
                LatestVersion = currentVersion,
                IsUpdateAvailable = false
            };
        }
    }
    public async Task InstallUpdateAsync(
    UpdateInfo updateInfo,
    CancellationToken cancellationToken = default)
    {
        if (!updateInfo.IsUpdateAvailable ||
            string.IsNullOrWhiteSpace(updateInfo.DownloadUrl))
        {
            throw new InvalidOperationException(
                "نسخه جدید قابل نصب نیست.");
        }

        var updaterPath = Path.Combine(
            AppContext.BaseDirectory,
            "Updater",
            "BarmanUpdater.exe");

        if (!File.Exists(updaterPath))
        {
            throw new FileNotFoundException(
                "BarmanUpdater.exe پیدا نشد.",
                updaterPath);
        }

        var zipPath = Path.Combine(
            Path.GetTempPath(),
            $"BarmanLims-{updateInfo.LatestVersion}.zip");

        await using (var responseStream =
            await _httpClient.GetStreamAsync(
                updateInfo.DownloadUrl,
                cancellationToken))
        await using (var fileStream =
            File.Create(zipPath))
        {
            await responseStream.CopyToAsync(
                fileStream,
                cancellationToken);
        }

        var appDirectory =
            AppContext.BaseDirectory;

        var processStartInfo = new ProcessStartInfo
        {
            FileName = updaterPath,
            Arguments =
                $"\"{zipPath}\" \"{appDirectory}\"",
            WorkingDirectory =
                Path.GetDirectoryName(updaterPath)!,
            UseShellExecute = true
        };

        Process.Start(processStartInfo);

        Environment.Exit(0);
    }
    private static string NormalizeVersion(string version)
    {
        version = version.Trim();

        if (version.StartsWith(
                "v",
                StringComparison.OrdinalIgnoreCase))
        {
            version = version[1..];
        }

        return version;
    }

    private static int CompareVersions(
        string latest,
        string current)
    {
        if (Version.TryParse(latest, out var latestVersion) &&
            Version.TryParse(current, out var currentVersion))
        {
            return latestVersion.CompareTo(currentVersion);
        }

        return string.Compare(
            latest,
            current,
            StringComparison.OrdinalIgnoreCase);
    }

    private static string? GetDownloadUrl(
        GitHubRelease release)
    {
        return release.Assets?
            .FirstOrDefault(x =>
                !string.IsNullOrWhiteSpace(x.BrowserDownloadUrl))
            ?.BrowserDownloadUrl;
    }

    private sealed class GitHubRelease
    {
        [JsonPropertyName("tag_name")]
        public string? TagName { get; set; }

        [JsonPropertyName("published_at")]
        public string? PublishedAt { get; set; }

        [JsonPropertyName("body")]
        public string? Body { get; set; }

        [JsonPropertyName("assets")]
        public List<GitHubAsset>? Assets { get; set; }
    }

    private sealed class GitHubAsset
    {
        [JsonPropertyName("browser_download_url")]
        public string? BrowserDownloadUrl { get; set; }
    }
}