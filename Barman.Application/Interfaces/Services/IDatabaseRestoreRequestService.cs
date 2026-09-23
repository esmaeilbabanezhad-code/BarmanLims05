namespace Barman.Application.Interfaces.Services;

public interface IDatabaseRestoreRequestService
{
    Task<string> CreateRequestAsync(
        DatabaseRestoreRequest request,
        CancellationToken cancellationToken = default);

    Task<DatabaseRestoreRequest?> GetRequestAsync(
        string requestPath,
        CancellationToken cancellationToken = default);

    Task UpdateRequestAsync(
        string requestPath,
        DatabaseRestoreRequest request,
        CancellationToken cancellationToken = default);
}