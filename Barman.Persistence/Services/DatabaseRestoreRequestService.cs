using Barman.Application.Interfaces.Services;
using System.Text.Json;

namespace Barman.Persistence.Services;

public sealed class DatabaseRestoreRequestService
    : IDatabaseRestoreRequestService
{
    private readonly JsonSerializerOptions _jsonOptions =
        new()
        {
            WriteIndented = true
        };

    private readonly string _restoreDirectory;

    public DatabaseRestoreRequestService()
    {
        _restoreDirectory =
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.CommonApplicationData),
                "BarmanLims",
                "Restore");

        Directory.CreateDirectory(
            _restoreDirectory);
    }

    public async Task<string> CreateRequestAsync(
        DatabaseRestoreRequest request,
        CancellationToken cancellationToken = default)
    {
        var requestPath =
            Path.Combine(
                _restoreDirectory,
                $"RestoreRequest_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}.json");

        request.CreatedAt =
            request.CreatedAt == default
                ? DateTime.Now
                : request.CreatedAt;

        request.Status = "Pending";

        var json =
            JsonSerializer.Serialize(
                request,
                _jsonOptions);

        await File.WriteAllTextAsync(
            requestPath,
            json,
            cancellationToken);

        return requestPath;
    }

    public async Task<DatabaseRestoreRequest?> GetRequestAsync(
        string requestPath,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(requestPath))
            return null;

        var json =
            await File.ReadAllTextAsync(
                requestPath,
                cancellationToken);

        return JsonSerializer.Deserialize<DatabaseRestoreRequest>(
            json,
            _jsonOptions);
    }

    public async Task UpdateRequestAsync(
        string requestPath,
        DatabaseRestoreRequest request,
        CancellationToken cancellationToken = default)
    {
        var json =
            JsonSerializer.Serialize(
                request,
                _jsonOptions);

        await File.WriteAllTextAsync(
            requestPath,
            json,
            cancellationToken);
    }
}