namespace Barman.Application.Versioning;

public interface IUpdateService
{
    Task<UpdateInfo> CheckForUpdateAsync(
        CancellationToken cancellationToken = default);

    Task InstallUpdateAsync(
        UpdateInfo updateInfo,
        CancellationToken cancellationToken = default);
}