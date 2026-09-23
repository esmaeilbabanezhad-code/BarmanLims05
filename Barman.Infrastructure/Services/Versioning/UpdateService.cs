using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Barman.Application.Versioning;

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